namespace TheSingularityWorkshop.MyVR;

/// <summary>
/// Boundary between MyVR's device-independent core and a concrete VR runtime.
/// Implementations own all platform/vendor API usage.
/// </summary>
public interface IDeviceRuntimeAdapter
{
    ValueTask<DeviceCapabilities> DetectCapabilitiesAsync(CancellationToken cancellationToken = default);
}
