using TheSingularityWorkshop.MyVR;

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
    public async Task Runtime_adapter_is_testable_without_hardware()
    {
        IDeviceRuntimeAdapter adapter = new FakeDeviceRuntimeAdapter(
            new DeviceCapabilities("fake.runtime", ["immersive.display"]));

        var result = await adapter.DetectCapabilitiesAsync();

        Assert.Equal("fake.runtime", result.RuntimeId);
        Assert.True(result.Supports("immersive.display"));
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
