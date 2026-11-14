namespace MyVR.Core;

/// <summary>Device-independent lifecycle states for an immersive manifestation session.</summary>
public enum MyVrSessionState
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
