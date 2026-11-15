namespace MyVR.Core;

/// <summary>
/// Identifies one immutable Experience artifact. MyVR sessions refer to this identity;
/// they never create a competing Experience identity.
/// </summary>
public sealed record ExperienceArtifactIdentity
{
    public ExperienceArtifactIdentity(string bundleId, string version, string contentHash)
    {
        BundleId = Require(bundleId, nameof(bundleId));
        Version = Require(version, nameof(version));
        ContentHash = Require(contentHash, nameof(contentHash)).ToLowerInvariant();

        if (ContentHash.Length != 64 || !ContentHash.All(Uri.IsHexDigit))
            throw new ArgumentException("ContentHash must be a 64-character SHA-256 hexadecimal digest.", nameof(contentHash));
    }

    public string BundleId { get; }
    public string Version { get; }
    public string ContentHash { get; }

    private static string Require(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", name)
            : value.Trim();
}
