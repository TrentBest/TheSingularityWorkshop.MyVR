namespace TheSingularityWorkshop.MyVR;

/// <summary>
/// Translates platform-neutral runtime and host lifecycle signals into the
/// canonical MyVR session state machine. Platform SDK types must stay outside
/// this class; a concrete host adapter is responsible for translating them.
/// </summary>
public sealed class DeviceSessionLifecycleBridge
{
    private readonly MyVrSession _session;
    private bool _runtimeConnected;
    private bool _runtimeReady;
    private bool _runtimeRunning;
    private bool _hostResumed;

    public DeviceSessionLifecycleBridge(MyVrSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public ImmersiveSessionState State => _session.State;

    /// <summary>
    /// Reports that the device runtime and its session resources are established.
    /// </summary>
    public void RuntimeConnected()
    {
        if (_runtimeConnected || _session.State != ImmersiveSessionState.Disconnected)
        {
            throw new InvalidOperationException("The device runtime is already connected.");
        }

        _runtimeConnected = true;
        _session.TransitionTo(ImmersiveSessionState.Discovering);
        _session.TransitionTo(ImmersiveSessionState.Connecting);
        _session.TransitionTo(ImmersiveSessionState.Connected);
        _session.TransitionTo(ImmersiveSessionState.Synchronizing);
    }

    /// <summary>
    /// Reports that the runtime session reached its ready-to-begin state.
    /// </summary>
    public void RuntimeReady()
    {
        EnsureRuntimeConnected();
        _runtimeReady = true;
        Reconcile();
    }

    /// <summary>
    /// Reports that the runtime successfully began frame-capable session work.
    /// </summary>
    public void RuntimeRunning()
    {
        EnsureRuntimeConnected();
        if (!_runtimeReady)
        {
            throw new InvalidOperationException("The runtime cannot run before it is ready.");
        }

        _runtimeRunning = true;
        Reconcile();
    }

    /// <summary>
    /// Reports that the runtime stopped its running session.
    /// </summary>
    public void RuntimeStopping()
    {
        _runtimeRunning = false;
        Reconcile();
    }

    /// <summary>
    /// Reports that the host activity is no longer available for presentation.
    /// The exact pre-suspension state is restored when the host resumes.
    /// </summary>
    public void HostPaused()
    {
        _hostResumed = false;

        if (_session.State is ImmersiveSessionState.Connected
            or ImmersiveSessionState.Synchronizing
            or ImmersiveSessionState.Ready
            or ImmersiveSessionState.Immersive)
        {
            _session.TransitionTo(ImmersiveSessionState.Suspended);
        }
    }

    /// <summary>
    /// Reports that the host activity can present again.
    /// Reconciliation prevents a stale running state after a runtime STOPPING event.
    /// </summary>
    public void HostResumed()
    {
        _hostResumed = true;

        if (_session.State == ImmersiveSessionState.Suspended)
        {
            _session.Resume();
        }

        Reconcile();
    }

    /// <summary>
    /// Reports terminal runtime/session loss. The Experience identity is retained,
    /// while the device session returns to Disconnected.
    /// </summary>
    public void RuntimeLost()
    {
        _runtimeConnected = false;
        _runtimeReady = false;
        _runtimeRunning = false;
        _session.Disconnect();
    }

    private void EnsureRuntimeConnected()
    {
        if (!_runtimeConnected)
        {
            throw new InvalidOperationException("The device runtime is not connected.");
        }
    }

    private void Reconcile()
    {
        if (_session.State == ImmersiveSessionState.Suspended || !_runtimeConnected)
        {
            return;
        }

        if (_runtimeReady && _session.State == ImmersiveSessionState.Synchronizing)
        {
            _session.TransitionTo(ImmersiveSessionState.Ready);
        }

        if (_runtimeRunning && _hostResumed && _session.State == ImmersiveSessionState.Ready)
        {
            _session.TransitionTo(ImmersiveSessionState.Immersive);
        }
        else if (!_runtimeRunning && _session.State == ImmersiveSessionState.Immersive)
        {
            _session.TransitionTo(ImmersiveSessionState.Ready);
        }
    }
}
