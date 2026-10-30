namespace TheSingularityWorkshop.MyVR;

/// <summary>
/// Lifecycle of one MyVR manifestation session.
/// </summary>
public enum ImmersiveSessionState
{
    Disconnected,
    Discovering,
    Connecting,
    Connected,
    Synchronizing,
    Ready,
    Immersive,
    Suspended
}
