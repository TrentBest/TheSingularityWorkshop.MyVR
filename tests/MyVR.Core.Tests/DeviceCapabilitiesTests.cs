using TheSingularityWorkshop.MyVR;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class DeviceCapabilitiesTests
{
    [Fact]
    public void Capability_snapshot_deduplicates_and_exposes_stable_values()
    {
        var capabilities = new DeviceCapabilities(
            "test.runtime",
            ["immersive.display", "spatial.input", "immersive.display"]);

        Assert.True(capabilities.Supports("immersive.display"));
        Assert.True(capabilities.Supports("spatial.input"));
        Assert.False(capabilities.Supports("hand.tracking"));
        Assert.Equal(
            new[] { "immersive.display", "spatial.input" },
            capabilities.SupportedCapabilities);
    }

    [Fact]
    public void Capability_snapshot_normalizes_identifiers_using_ordinal_matching()
    {
        var capabilities = new DeviceCapabilities(
            " test.runtime ",
            [" spatial.input ", "Immersive.Display"]);

        Assert.Equal("test.runtime", capabilities.RuntimeId);
        Assert.True(capabilities.Supports("spatial.input"));
        Assert.True(capabilities.Supports("Immersive.Display"));
        Assert.False(capabilities.Supports("immersive.display"));
    }

    [Fact]
    public void Capability_snapshot_does_not_expose_mutable_backing_storage()
    {
        var source = new List<string> { "immersive.display" };
        var capabilities = new DeviceCapabilities("test.runtime", source);

        source.Add("spatial.input");

        Assert.True(capabilities.Supports("immersive.display"));
        Assert.False(capabilities.Supports("spatial.input"));
        Assert.Throws<NotSupportedException>(() =>
            ((IList<string>)capabilities.SupportedCapabilities).Add("hand.tracking"));
    }

    [Fact]
    public async Task Runtime_adapter_is_testable_without_hardware()
    {
        IDeviceRuntimeAdapter adapter = new FakeDeviceRuntimeAdapter(
            new DeviceCapabilities("fake.runtime", ["immersive.display"]));

        var result = await adapter.DetectCapabilitiesAsync();

        Assert.Equal("fake.runtime", result.RuntimeId);
        Assert.True(result.Supports("immersive.display"));
    }

    [Fact]
    public async Task Runtime_adapter_observes_cancellation()
    {
        IDeviceRuntimeAdapter adapter = new FakeDeviceRuntimeAdapter(
            new DeviceCapabilities("fake.runtime", ["immersive.display"]));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => adapter.DetectCapabilitiesAsync(cancellation.Token).AsTask());
    }

    private sealed class FakeDeviceRuntimeAdapter(DeviceCapabilities capabilities) : IDeviceRuntimeAdapter
    {
        public ValueTask<DeviceCapabilities> DetectCapabilitiesAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(capabilities);
        }
    }
}
