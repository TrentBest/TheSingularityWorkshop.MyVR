namespace TheSingularityWorkshop.MyVR;

/// <summary>
/// Identifies one immutable published Experience artifact.
/// Session identity is deliberately represented separately by <see cref="MyVrSession.SessionId"/>.
/// </summary>
public sealed record ExperienceIdentity
{
    public string BundleId { get; }
    public string Version { get; }
    public string ContentHash { get; }

    public ExperienceIdentity(string bundleId, string version, string contentHash)
    {
        BundleId = RequireValue(bundleId, nameof(bundleId));
        Version = RequireValue(version, nameof(version));
        ContentHash = RequireValue(contentHash, nameof(contentHash));
    }

    private static string RequireValue(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
