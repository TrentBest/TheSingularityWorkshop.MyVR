using TheSingularityWorkshop.MyVR;

namespace MyVR.Core.Tests;

public sealed class MyVrSessionTests
{
    private static ExperienceIdentity CreateExperience() =>
        new("experience.demo", "1.2.3", "sha256:0123456789abcdef");

    [Fact]
    public void Session_has_identity_separate_from_experience_identity()
    {
        var experience = CreateExperience();
        var sessionId = Guid.NewGuid();
        var session = new MyVrSession(experience, sessionId);

        Assert.Same(experience, session.Experience);
        Assert.Equal(sessionId, session.SessionId);
        Assert.NotEqual(Guid.Empty, session.SessionId);
        Assert.Equal(ImmersiveSessionState.Disconnected, session.State);
    }

    [Fact]
    public void Session_accepts_the_expected_startup_path()
    {
        var session = new MyVrSession(CreateExperience());

        session.TransitionTo(ImmersiveSessionState.Discovering);
        session.TransitionTo(ImmersiveSessionState.Connecting);
        session.TransitionTo(ImmersiveSessionState.Connected);
        session.TransitionTo(ImmersiveSessionState.Synchronizing);
        session.TransitionTo(ImmersiveSessionState.Ready);
        session.TransitionTo(ImmersiveSessionState.Immersive);

        Assert.Equal(ImmersiveSessionState.Immersive, session.State);
    }

    [Fact]
    public void Session_rejects_skipped_or_invalid_transitions()
    {
        var session = new MyVrSession(CreateExperience());

        Assert.Throws<InvalidOperationException>(() =>
            session.TransitionTo(ImmersiveSessionState.Immersive));
        Assert.Throws<InvalidOperationException>(() =>
            session.TransitionTo(ImmersiveSessionState.Ready));
        Assert.Equal(ImmersiveSessionState.Disconnected, session.State);
    }

    [Theory]
    [InlineData(ImmersiveSessionState.Connected)]
    [InlineData(ImmersiveSessionState.Synchronizing)]
    [InlineData(ImmersiveSessionState.Ready)]
    [InlineData(ImmersiveSessionState.Immersive)]
    public void Suspend_and_resume_restore_the_prior_state(ImmersiveSessionState priorState)
    {
        var session = CreateSessionAt(priorState);

        session.TransitionTo(ImmersiveSessionState.Suspended);
        Assert.Equal(ImmersiveSessionState.Suspended, session.State);

        session.Resume();

        Assert.Equal(priorState, session.State);
    }

    [Fact]
    public void Disconnect_from_suspended_state_preserves_both_identities()
    {
        var experience = CreateExperience();
        var sessionId = Guid.NewGuid();
        var session = new MyVrSession(experience, sessionId);
        session.TransitionTo(ImmersiveSessionState.Discovering);
        session.TransitionTo(ImmersiveSessionState.Connecting);
        session.TransitionTo(ImmersiveSessionState.Connected);
        session.TransitionTo(ImmersiveSessionState.Suspended);

        session.Disconnect();
        session.Disconnect();

        Assert.Equal(ImmersiveSessionState.Disconnected, session.State);
        Assert.Same(experience, session.Experience);
        Assert.Equal(sessionId, session.SessionId);
    }

    [Fact]
    public void Session_can_reconnect_after_disconnect()
    {
        var session = CreateSessionAt(ImmersiveSessionState.Ready);
        session.Disconnect();

        session.TransitionTo(ImmersiveSessionState.Discovering);
        session.TransitionTo(ImmersiveSessionState.Connecting);
        session.TransitionTo(ImmersiveSessionState.Connected);

        Assert.Equal(ImmersiveSessionState.Connected, session.State);
    }

    private static MyVrSession CreateSessionAt(ImmersiveSessionState target)
    {
        var session = new MyVrSession(CreateExperience());
        var path = new[]
        {
            ImmersiveSessionState.Discovering,
            ImmersiveSessionState.Connecting,
            ImmersiveSessionState.Connected,
            ImmersiveSessionState.Synchronizing,
            ImmersiveSessionState.Ready,
            ImmersiveSessionState.Immersive
        };

        foreach (var state in path.TakeWhile(state => state != target).Append(target))
        {
            if (state != session.State)
            {
                session.TransitionTo(state);
            }
        }

        return session;
    }
}
