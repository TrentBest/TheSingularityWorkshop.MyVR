namespace TheSingularityWorkshop.MyVR;

/// <summary>
/// Tracks a MyVR session independently from the immutable Experience it manifests.
/// It owns lifecycle state only; it does not compose or execute the Experience.
/// </summary>
public sealed class MyVrSession
{
    private static readonly IReadOnlyDictionary<ImmersiveSessionState, HashSet<ImmersiveSessionState>> AllowedTransitions =
        new Dictionary<ImmersiveSessionState, HashSet<ImmersiveSessionState>>
        {
            [ImmersiveSessionState.Disconnected] = [ImmersiveSessionState.Discovering],
            [ImmersiveSessionState.Discovering] = [ImmersiveSessionState.Connecting, ImmersiveSessionState.Disconnected],
            [ImmersiveSessionState.Connecting] = [ImmersiveSessionState.Connected, ImmersiveSessionState.Disconnected],
            [ImmersiveSessionState.Connected] = [ImmersiveSessionState.Synchronizing, ImmersiveSessionState.Suspended, ImmersiveSessionState.Disconnected],
            [ImmersiveSessionState.Synchronizing] = [ImmersiveSessionState.Ready, ImmersiveSessionState.Suspended, ImmersiveSessionState.Disconnected],
            [ImmersiveSessionState.Ready] = [ImmersiveSessionState.Immersive, ImmersiveSessionState.Suspended, ImmersiveSessionState.Disconnected],
            [ImmersiveSessionState.Immersive] = [ImmersiveSessionState.Ready, ImmersiveSessionState.Suspended, ImmersiveSessionState.Disconnected],
            [ImmersiveSessionState.Suspended] = [ImmersiveSessionState.Disconnected]
        };

    private ImmersiveSessionState? _stateBeforeSuspension;

    public Guid SessionId { get; }
    public ExperienceIdentity Experience { get; }
    public ImmersiveSessionState State { get; private set; } = ImmersiveSessionState.Disconnected;

    public MyVrSession(ExperienceIdentity experience, Guid? sessionId = null)
    {
        ArgumentNullException.ThrowIfNull(experience);
        Experience = experience;
        SessionId = sessionId ?? Guid.NewGuid();

        if (SessionId == Guid.Empty)
        {
            throw new ArgumentException("A session ID must not be empty.", nameof(sessionId));
        }
    }

    public void TransitionTo(ImmersiveSessionState next)
    {
        if (next == ImmersiveSessionState.Suspended)
        {
            if (State is not (ImmersiveSessionState.Connected
                or ImmersiveSessionState.Synchronizing
                or ImmersiveSessionState.Ready
                or ImmersiveSessionState.Immersive))
            {
                throw InvalidTransition(next);
            }

            _stateBeforeSuspension = State;
            State = ImmersiveSessionState.Suspended;
            return;
        }

        if (State == ImmersiveSessionState.Suspended)
        {
            throw new InvalidOperationException("A suspended session must be resumed or disconnected.");
        }

        if (!AllowedTransitions[State].Contains(next))
        {
            throw InvalidTransition(next);
        }

        State = next;
    }

    /// <summary>
    /// Restores the exact lifecycle state that was active before suspension.
    /// </summary>
    public void Resume()
    {
        if (State != ImmersiveSessionState.Suspended || _stateBeforeSuspension is null)
        {
            throw new InvalidOperationException("Only a suspended session can be resumed.");
        }

        State = _stateBeforeSuspension.Value;
        _stateBeforeSuspension = null;
    }

    /// <summary>
    /// Ends the device session without changing its Experience or session identity.
    /// Repeated disconnects are harmless.
    /// </summary>
    public void Disconnect()
    {
        State = ImmersiveSessionState.Disconnected;
        _stateBeforeSuspension = null;
    }

    private InvalidOperationException InvalidTransition(ImmersiveSessionState next) =>
        new($"Cannot transition a session from {State} to {next}.");
}
