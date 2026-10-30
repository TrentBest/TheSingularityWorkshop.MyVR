namespace TheSingularityWorkshop.MyVR;

/// <summary>
/// Immutable capabilities reported by a device/runtime adapter.
/// Capability names are supplied by the adapter; this core does not impose a vendor or engine taxonomy.
/// </summary>
public sealed class DeviceCapabilities
{
    private readonly HashSet<string> _supported;
    private readonly IReadOnlyCollection<string> _readOnlySupported;

    public string RuntimeId { get; }

    public IReadOnlyCollection<string> SupportedCapabilities => _readOnlySupported;

    public DeviceCapabilities(string runtimeId, IEnumerable<string> supportedCapabilities)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(runtimeId);
        ArgumentNullException.ThrowIfNull(supportedCapabilities);

        RuntimeId = runtimeId.Trim();
        _supported = new HashSet<string>(StringComparer.Ordinal);

        foreach (var capability in supportedCapabilities)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(capability);
            _supported.Add(capability.Trim());
        }

        _readOnlySupported = Array.AsReadOnly(_supported.Order(StringComparer.Ordinal).ToArray());
    }

    public bool Supports(string capability)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(capability);
        return _supported.Contains(capability.Trim());
    }
}
