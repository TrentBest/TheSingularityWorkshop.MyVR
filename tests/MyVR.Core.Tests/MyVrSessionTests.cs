using MyVR.Core;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class MyVrSessionTests
{
    private static ExperienceArtifactIdentity Artifact() =>
        new("com.singularityworkshop.lenny", "1.0.0", new string('a', 64));

    [Fact]
    public void SessionRetainsCanonicalArtifactIdentityAndGetsIndependentSessionIdentity()
    {
        var artifact = Artifact();
        var session = new MyVrSession(artifact);

        Assert.Equal(artifact, session.Artifact);
        Assert.NotEqual(Guid.Empty, session.SessionId);
        Assert.Equal(MyVrSessionState.Disconnected, session.State);
    }

    [Fact]
    public void SessionCanReachImmersiveOnlyThroughReady()
    {
        var session = new MyVrSession(Artifact());

        session.TransitionTo(MyVrSessionState.Discovering);
        session.TransitionTo(MyVrSessionState.Connecting);
        session.TransitionTo(MyVrSessionState.Connected);
        session.TransitionTo(MyVrSessionState.Synchronizing);
        session.TransitionTo(MyVrSessionState.Ready);
        session.TransitionTo(MyVrSessionState.Immersive);

        Assert.Equal(MyVrSessionState.Immersive, session.State);
    }

    [Fact]
    public void SessionRejectsSkippedOrInvalidTransitions()
    {
        var session = new MyVrSession(Artifact());

        Assert.Throws<InvalidOperationException>(() => session.TransitionTo(MyVrSessionState.Immersive));
        Assert.Throws<InvalidOperationException>(() => session.TransitionTo((MyVrSessionState)999));
        Assert.Equal(MyVrSessionState.Disconnected, session.State);
    }

    [Fact]
    public void SuspendedSessionMustSynchronizeBeforeBecomingReadyAgain()
    {
        var session = new MyVrSession(Artifact());
        session.TransitionTo(MyVrSessionState.Discovering);
        session.TransitionTo(MyVrSessionState.Connecting);
        session.TransitionTo(MyVrSessionState.Connected);
        session.TransitionTo(MyVrSessionState.Synchronizing);
        session.TransitionTo(MyVrSessionState.Ready);
        session.TransitionTo(MyVrSessionState.Suspended);

        Assert.Throws<InvalidOperationException>(() => session.TransitionTo(MyVrSessionState.Immersive));
        session.TransitionTo(MyVrSessionState.Synchronizing);
        session.TransitionTo(MyVrSessionState.Ready);
        Assert.Equal(MyVrSessionState.Ready, session.State);
    }

    [Theory]
    [InlineData("", "1.0.0", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("bundle", "", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("bundle", "1.0.0", "not-a-sha256")]
    public void ArtifactIdentityRejectsMalformedComponents(string bundleId, string version, string hash)
    {
        Assert.Throws<ArgumentException>(() => new ExperienceArtifactIdentity(bundleId, version, hash));
    }
}
