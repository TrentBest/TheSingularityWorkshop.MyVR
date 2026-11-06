using TheSingularityWorkshop.MyVR;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class DeviceSessionLifecycleBridgeTests
{
    private static ExperienceIdentity CreateExperience() =>
        new("experience.demo", "1.2.3", "sha256:0123456789abcdef");

    private static DeviceSessionLifecycleBridge CreateConnectedBridge(out MyVrSession session)
    {
        session = new MyVrSession(CreateExperience());
        var bridge = new DeviceSessionLifecycleBridge(session);
        bridge.RuntimeConnected();
        return bridge;
    }

    [Fact]
    public void Runtime_connection_advances_session_to_synchronizing()
    {
        var bridge = CreateConnectedBridge(out _);

        Assert.Equal(ImmersiveSessionState.Synchronizing, bridge.State);
    }

    [Fact]
    public void Runtime_ready_advances_session_to_ready()
    {
        var bridge = CreateConnectedBridge(out _);

        bridge.RuntimeReady();

        Assert.Equal(ImmersiveSessionState.Ready, bridge.State);
    }

    [Fact]
    public void Runtime_running_enters_immersive_only_when_host_is_resumed()
    {
        var bridge = CreateConnectedBridge(out _);
        bridge.RuntimeReady();

        bridge.RuntimeRunning();
        Assert.Equal(ImmersiveSessionState.Ready, bridge.State);

        bridge.HostResumed();

        Assert.Equal(ImmersiveSessionState.Immersive, bridge.State);
    }

    [Fact]
    public void Host_resume_and_pause_restore_the_exact_runtime_state()
    {
        var bridge = CreateConnectedBridge(out _);
        bridge.RuntimeReady();
        bridge.HostResumed();
        bridge.RuntimeRunning();

        bridge.HostPaused();
        Assert.Equal(ImmersiveSessionState.Suspended, bridge.State);

        bridge.HostResumed();

        Assert.Equal(ImmersiveSessionState.Immersive, bridge.State);
    }

    [Fact]
    public void Runtime_stopping_while_host_is_paused_does_not_restore_stale_immersive_state()
    {
        var bridge = CreateConnectedBridge(out _);
        bridge.RuntimeReady();
        bridge.HostResumed();
        bridge.RuntimeRunning();
        bridge.HostPaused();

        bridge.RuntimeStopping();
        bridge.HostResumed();

        Assert.Equal(ImmersiveSessionState.Ready, bridge.State);
    }

    [Fact]
    public void Runtime_stopping_returns_active_session_to_ready()
    {
        var bridge = CreateConnectedBridge(out _);
        bridge.RuntimeReady();
        bridge.HostResumed();
        bridge.RuntimeRunning();

        bridge.RuntimeStopping();

        Assert.Equal(ImmersiveSessionState.Ready, bridge.State);
    }

    [Fact]
    public void Runtime_loss_disconnects_session_without_changing_either_identity()
    {
        var bridge = CreateConnectedBridge(out var session);
        var experience = session.Experience;
        var sessionId = session.SessionId;
        bridge.RuntimeReady();
        bridge.HostResumed();
        bridge.RuntimeRunning();

        bridge.RuntimeLost();

        Assert.Equal(ImmersiveSessionState.Disconnected, bridge.State);
        Assert.Same(experience, session.Experience);
        Assert.Equal(sessionId, session.SessionId);
    }

    [Fact]
    public void Runtime_ready_is_rejected_before_connection()
    {
        var session = new MyVrSession(CreateExperience());
        var bridge = new DeviceSessionLifecycleBridge(session);

        Assert.Throws<InvalidOperationException>(() => bridge.RuntimeReady());
        Assert.Equal(ImmersiveSessionState.Disconnected, bridge.State);
    }

    [Fact]
    public void Runtime_running_is_rejected_before_ready()
    {
        var bridge = CreateConnectedBridge(out _);

        Assert.Throws<InvalidOperationException>(() => bridge.RuntimeRunning());
        Assert.Equal(ImmersiveSessionState.Synchronizing, bridge.State);
    }

    [Fact]
    public void Host_pause_before_connection_is_harmless()
    {
        var session = new MyVrSession(CreateExperience());
        var bridge = new DeviceSessionLifecycleBridge(session);

        bridge.HostPaused();

        Assert.Equal(ImmersiveSessionState.Disconnected, bridge.State);
    }

    [Fact]
    public void Runtime_can_reconnect_after_terminal_loss()
    {
        var bridge = CreateConnectedBridge(out _);
        bridge.RuntimeReady();
        bridge.RuntimeLost();

        bridge.RuntimeConnected();

        Assert.Equal(ImmersiveSessionState.Synchronizing, bridge.State);
    }
}
