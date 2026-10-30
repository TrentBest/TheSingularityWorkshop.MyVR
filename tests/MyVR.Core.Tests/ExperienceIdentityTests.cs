using TheSingularityWorkshop.MyVR;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class ExperienceIdentityTests
{
    [Fact]
    public void Identity_requires_all_three_artifact_identity_parts()
    {
        Assert.Throws<ArgumentException>(() => new ExperienceIdentity("", "1.0.0", "hash"));
        Assert.Throws<ArgumentException>(() => new ExperienceIdentity("bundle", " ", "hash"));
        Assert.Throws<ArgumentException>(() => new ExperienceIdentity("bundle", "1.0.0", ""));
    }

    [Fact]
    public void Identity_trims_values_and_compares_by_artifact_identity()
    {
        var first = new ExperienceIdentity(" bundle ", " 1.0.0 ", " sha256:abc ");
        var second = new ExperienceIdentity("bundle", "1.0.0", "sha256:abc");

        Assert.Equal("bundle", first.BundleId);
        Assert.Equal("1.0.0", first.Version);
        Assert.Equal("sha256:abc", first.ContentHash);
        Assert.Equal(first, second);
    }
}
