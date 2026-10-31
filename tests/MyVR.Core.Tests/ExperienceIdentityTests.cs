using TheSingularityWorkshop.MyVR;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class ExperienceIdentityTests
{
    [Fact]
    public void Identity_normalizes_values_and_compares_by_artifact_identity()
    {
        var first = new ExperienceIdentity(
            " experience.demo ",
            " 1.2.3 ",
            " sha256:0123456789abcdef ");
        var second = new ExperienceIdentity(
            "experience.demo",
            "1.2.3",
            "sha256:0123456789abcdef");

        Assert.Equal("experience.demo", first.BundleId);
        Assert.Equal("1.2.3", first.Version);
        Assert.Equal("sha256:0123456789abcdef", first.ContentHash);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Identity_rejects_blank_bundle_ids(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            new ExperienceIdentity(value, "1.2.3", "sha256:0123456789abcdef"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Identity_rejects_blank_versions(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            new ExperienceIdentity("experience.demo", value, "sha256:0123456789abcdef"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Identity_rejects_blank_content_hashes(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            new ExperienceIdentity("experience.demo", "1.2.3", value));
    }
}
