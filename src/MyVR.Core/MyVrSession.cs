namespace MyVR.Core;

/// <summary>
/// Tracks the local MyVR session lifecycle for one immutable Experience artifact.
/// This core contains no headset SDK, transport, renderer, or Experience-specific story model.
/// </summary>
public sealed class MyVrSession
{
    private static readonly IReadOnlyDictionary<MyVrSessionState, MyVrSessionState[]> AllowedTransitions =
        new Dictionary<MyVrSessionState, MyVrSessionState[]>
        {
            [MyVrSessionState.Disconnected] = [MyVrSessionState.Discovering],
            [MyVrSessionState.Discovering] = [MyVrSessionState.Connecting, MyVrSessionState.Disconnected],
            [MyVrSessionState.Connecting] = [MyVrSessionState.Connected, MyVrSessionState.Disconnected],
            [MyVrSessionState.Connected] = [MyVrSessionState.Synchronizing, MyVrSessionState.Suspended, MyVrSessionState.Disconnected],
            [MyVrSessionState.Synchronizing] = [MyVrSessionState.Ready, MyVrSessionState.Suspended, MyVrSessionState.Disconnected],
            [MyVrSessionState.Ready] = [MyVrSessionState.Immersive, MyVrSessionState.Suspended, MyVrSessionState.Disconnected],
            [MyVrSessionState.Immersive] = [MyVrSessionState.Ready, MyVrSessionState.Suspended, MyVrSessionState.Disconnected],
            [MyVrSessionState.Suspended] = [MyVrSessionState.Synchronizing, MyVrSessionState.Disconnected]
        };

    public MyVrSession(ExperienceArtifactIdentity artifact)
    {
        Artifact = artifact ?? throw new ArgumentNullException(nameof(artifact));
        SessionId = Guid.NewGuid();
        State = MyVrSessionState.Disconnected;
    }

    public Guid SessionId { get; }
    public ExperienceArtifactIdentity Artifact { get; }
    public MyVrSessionState State { get; private set; }

    /// <summary>Moves the session only along explicitly supported lifecycle edges.</summary>
    public void TransitionTo(MyVrSessionState next)
    {
        if (!AllowedTransitions.TryGetValue(State, out var allowed) || !allowed.Contains(next))
            throw new InvalidOperationException($"MyVR session cannot transition from {State} to {next}.");

        State = next;
    }
}
