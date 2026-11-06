using TheSingularityWorkshop.MyVR.Tactics;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class TacticalContractsTests
{
    private static TacticalTarget Target() => new("target-1", "world", 10, 0, 20);

    [Fact]
    public void Intent_keeps_player_goal_separate_from_bot_plan()
    {
        var intent = new TacticalIntent(
            TacticalIntentKind.SynchronizedAttack,
            Target(),
            intentId: Guid.NewGuid(),
            issuedAt: DateTimeOffset.UnixEpoch);

        Assert.Equal(TacticalIntentKind.SynchronizedAttack, intent.Kind);
        Assert.Equal("target-1", intent.Target!.TargetId);
        Assert.Equal(DateTimeOffset.UnixEpoch, intent.IssuedAt);
    }

    [Fact]
    public void Target_rejects_non_finite_coordinates()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TacticalTarget("target-1", "world", double.NaN, 0, 0));
    }

    [Fact]
    public void Targeted_intents_require_a_target()
    {
        Assert.Throws<ArgumentException>(() =>
            new TacticalIntent(TacticalIntentKind.AttackPosition));
    }

    [Fact]
    public void Split_intent_requires_a_positive_group_count()
    {
        Assert.Throws<ArgumentException>(() =>
            new TacticalIntent(TacticalIntentKind.SplitIntoGroups));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TacticalIntent(TacticalIntentKind.SplitIntoGroups, requestedGroupCount: 0));
    }

    [Fact]
    public void Proposal_requires_unique_groups_and_explicit_synchronization()
    {
        var proposal = new TacticalPlanProposal(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Two groups stage on separate routes and coordinate arrival.",
            new[]
            {
                new TacticalGroupPlan("left", "Left flank", "Western approach"),
                new TacticalGroupPlan("right", "Right flank", "Eastern approach")
            },
            new TacticalSynchronization(
                TacticalSynchronizationKind.CoordinatedArrival,
                TimeSpan.FromMilliseconds(250)));

        proposal.Validate();

        Assert.Equal(TacticalSynchronizationKind.CoordinatedArrival, proposal.Synchronization!.Kind);
        Assert.Equal(TimeSpan.FromMilliseconds(250), proposal.Synchronization.ArrivalTolerance);
    }

    [Fact]
    public void Proposal_rejects_duplicate_group_ids()
    {
        var proposal = new TacticalPlanProposal(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Invalid duplicate groups.",
            new[]
            {
                new TacticalGroupPlan("same", "Left", "West"),
                new TacticalGroupPlan("same", "Right", "East")
            });

        Assert.Throws<InvalidOperationException>(proposal.Validate);
    }

    [Fact]
    public void Player_decision_and_execution_state_are_separate_values()
    {
        var response = new TacticalPlanResponse(Guid.NewGuid(), TacticalPlanDecision.Approve);
        var execution = new TacticalExecutionSnapshot(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TacticalExecutionState.Staging);

        Assert.Equal(TacticalPlanDecision.Approve, response.Decision);
        Assert.Equal(TacticalExecutionState.Staging, execution.State);
    }
}
