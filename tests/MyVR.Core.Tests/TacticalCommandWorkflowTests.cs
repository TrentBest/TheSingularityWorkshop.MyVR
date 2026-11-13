using TheSingularityWorkshop.MyVR.Tactics;
using Xunit;

namespace MyVR.Core.Tests;

public sealed class TacticalCommandWorkflowTests
{
    private static readonly DateTimeOffset FixedTime = DateTimeOffset.UnixEpoch;

    [Fact]
    public async Task Intent_proposal_approval_and_execution_are_recorded_with_provenance()
    {
        var adapter = new DeterministicTacticalAdapter();
        var workflow = new TacticalCommandWorkflow(adapter, () => FixedTime);
        var intent = new TacticalIntent(TacticalIntentKind.FlankLeft, intentId: Guid.NewGuid());
        var proposed = await workflow.SubmitIntentAsync(intent);
        Assert.Equal(intent.IntentId, proposed.Proposal.IntentId);
        Assert.Null(proposed.Execution);
        Assert.Collection(proposed.LearningEvents,
            item => AssertEvent(item, "intent.submitted", "player"),
            item => AssertEvent(item, "plan.proposed", "bot"));

        var result = await workflow.RespondAsync(
            new TacticalPlanResponse(proposed.Proposal.ProposalId, TacticalPlanDecision.Approve));
        Assert.Equal(TacticalPlanDecision.Approve, result.Decision);
        Assert.Equal(TacticalExecutionState.Executing, result.Execution!.State);
        Assert.Contains(result.LearningEvents, item => item.EventType == "plan.approve" && item.Actor == "player");
        Assert.Contains(result.LearningEvents, item => item.EventType == "execution.executing" && item.Actor == "adapter");
        Assert.All(result.LearningEvents, item => Assert.Equal(FixedTime, item.OccurredAt));
    }

    [Fact]
    public async Task Modified_plan_is_forwarded_and_rejection_never_starts_execution()
    {
        var adapter = new DeterministicTacticalAdapter();
        var workflow = new TacticalCommandWorkflow(adapter, () => FixedTime);
        var proposed = await workflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.FlankRight));
        var modifiedGroups = new[] { new TacticalGroupPlan("player-left", "Screen", "Northern route") };
        var result = await workflow.RespondAsync(new TacticalPlanResponse(
            proposed.Proposal.ProposalId, TacticalPlanDecision.Modify, modifiedGroups));
        Assert.Equal(modifiedGroups, adapter.LastResponse!.ModifiedGroups);
        Assert.NotNull(result.Execution);

        var rejectingAdapter = new DeterministicTacticalAdapter();
        var rejectingWorkflow = new TacticalCommandWorkflow(rejectingAdapter, () => FixedTime);
        var rejectedProposal = await rejectingWorkflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.Hold));
        var rejected = await rejectingWorkflow.RespondAsync(new TacticalPlanResponse(
            rejectedProposal.Proposal.ProposalId, TacticalPlanDecision.Reject));
        Assert.Null(rejected.Execution);
        Assert.Equal(0, rejectingAdapter.ResponseCount);
        Assert.Contains(rejected.LearningEvents, item => item.EventType == "plan.reject" && item.Actor == "player");
    }

    [Fact]
    public async Task Execution_progress_and_successful_completion_are_recorded_in_revision_order()
    {
        var workflow = new TacticalCommandWorkflow(new DeterministicTacticalAdapter(), () => FixedTime);
        var proposal = await workflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.FlankLeft));
        var started = await workflow.RespondAsync(new TacticalPlanResponse(
            proposal.Proposal.ProposalId, TacticalPlanDecision.Approve));

        var staging = await workflow.ObserveExecutionAsync(new TacticalExecutionSnapshot(
            started.Execution!.ExecutionId, started.Intent.IntentId, TacticalExecutionState.Staging, "Groups assembling.", 1));
        var ready = await workflow.ObserveExecutionAsync(new TacticalExecutionSnapshot(
            started.Execution.ExecutionId, started.Intent.IntentId, TacticalExecutionState.Ready, "Both groups ready.", 2));
        var executing = await workflow.ObserveExecutionAsync(new TacticalExecutionSnapshot(
            started.Execution.ExecutionId, started.Intent.IntentId, TacticalExecutionState.Executing, "Maneuver released.", 3));
        var completed = await workflow.ObserveExecutionAsync(new TacticalExecutionSnapshot(
            started.Execution.ExecutionId, started.Intent.IntentId, TacticalExecutionState.Completed, "Objective reached.", 4));

        Assert.Equal(TacticalExecutionState.Completed, completed.Execution!.State);
        Assert.Equal(new[] { "execution.executing", "execution.staging", "execution.ready",
            "execution.executing", "execution.completed" },
            completed.LearningEvents.Where(e => e.EventType.StartsWith("execution.", StringComparison.Ordinal))
                .Select(e => e.EventType));
        Assert.Equal(4, completed.Execution.Revision);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await workflow.ObserveExecutionAsync(new TacticalExecutionSnapshot(
                completed.Execution.ExecutionId, completed.Intent.IntentId, TacticalExecutionState.Failed, "Late failure", 5)));
    }

    [Fact]
    public async Task Execution_failure_is_recorded_as_a_terminal_outcome()
    {
        var workflow = new TacticalCommandWorkflow(new DeterministicTacticalAdapter(), () => FixedTime);
        var proposal = await workflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.AttackPosition,
            new TacticalTarget("target", "world", 1, 2, 3)));
        var started = await workflow.RespondAsync(new TacticalPlanResponse(
            proposal.Proposal.ProposalId, TacticalPlanDecision.Approve));
        var failed = await workflow.ObserveExecutionAsync(new TacticalExecutionSnapshot(
            started.Execution!.ExecutionId, started.Intent.IntentId, TacticalExecutionState.Failed,
            "Bot reported an unrecoverable execution failure.", 1));

        Assert.Equal(TacticalExecutionState.Failed, failed.Execution!.State);
        Assert.Contains(failed.LearningEvents, e => e.EventType == "execution.failed" && e.Actor == "adapter");
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.CancelAsync());
    }

    [Fact]
    public async Task Execution_updates_reject_wrong_correlation_stale_revisions_and_illegal_transitions()
    {
        var workflow = new TacticalCommandWorkflow(new DeterministicTacticalAdapter(), () => FixedTime);
        var proposal = await workflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.Regroup));
        var started = await workflow.RespondAsync(new TacticalPlanResponse(
            proposal.Proposal.ProposalId, TacticalPlanDecision.Approve));
        var current = started.Execution!;

        await Assert.ThrowsAsync<ArgumentException>(async () => await workflow.ObserveExecutionAsync(
            new TacticalExecutionSnapshot(current.ExecutionId, Guid.NewGuid(), TacticalExecutionState.Staging, Revision: 1)));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.ObserveExecutionAsync(
            new TacticalExecutionSnapshot(current.ExecutionId, current.IntentId, TacticalExecutionState.Staging, Revision: 0)));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.ObserveExecutionAsync(
            new TacticalExecutionSnapshot(current.ExecutionId, current.IntentId, TacticalExecutionState.Completed, Revision: 1)));
    }

    [Fact]
    public async Task Cancellation_requires_revision_ordered_adapter_confirmation()
    {
        var workflow = new TacticalCommandWorkflow(new DeterministicTacticalAdapter(), () => FixedTime);
        var proposal = await workflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.Hold));
        await workflow.RespondAsync(new TacticalPlanResponse(proposal.Proposal.ProposalId, TacticalPlanDecision.Approve));
        var cancelled = await workflow.CancelAsync();

        Assert.Equal(TacticalExecutionState.Cancelled, cancelled.Execution!.State);
        Assert.Equal(1, cancelled.Execution.Revision);
        Assert.Contains(cancelled.LearningEvents, item => item.EventType == "execution.cancelled" && item.Actor == "adapter");
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.CancelAsync());
    }

    [Fact]
    public async Task Workflow_rejects_unrelated_proposal_and_duplicate_player_decision()
    {
        var workflow = new TacticalCommandWorkflow(new DeterministicTacticalAdapter(), () => FixedTime);
        var proposed = await workflow.SubmitIntentAsync(new TacticalIntent(TacticalIntentKind.Regroup));
        await Assert.ThrowsAsync<ArgumentException>(async () => await workflow.RespondAsync(
            new TacticalPlanResponse(Guid.NewGuid(), TacticalPlanDecision.Approve)));
        await workflow.RespondAsync(new TacticalPlanResponse(proposed.Proposal.ProposalId, TacticalPlanDecision.Reject));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.RespondAsync(
            new TacticalPlanResponse(proposed.Proposal.ProposalId, TacticalPlanDecision.Approve)));
    }

    private static void AssertEvent(TacticalLearningEvent item, string eventType, string actor)
    {
        Assert.Equal(eventType, item.EventType);
        Assert.Equal(actor, item.Actor);
        Assert.NotEqual(Guid.Empty, item.EventId);
    }

    private sealed class DeterministicTacticalAdapter : ITacticalCommandAdapter
    {
        private readonly Guid _executionId = Guid.NewGuid();
        private Guid _intentId;
        public TacticalPlanResponse? LastResponse { get; private set; }
        public int ResponseCount { get; private set; }

        public ValueTask<TacticalPlanProposal> ProposeAsync(TacticalIntent intent,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _intentId = intent.IntentId;
            return ValueTask.FromResult(new TacticalPlanProposal(Guid.NewGuid(), intent.IntentId,
                "Deterministic two-route proposal.", new[]
                {
                    new TacticalGroupPlan("group-a", "Primary", "Route A"),
                    new TacticalGroupPlan("group-b", "Support", "Route B")
                }, intent.Kind == TacticalIntentKind.SynchronizedAttack
                    ? new TacticalSynchronization(TacticalSynchronizationKind.SharedRelease, TimeSpan.Zero)
                    : null));
        }

        public ValueTask<TacticalExecutionSnapshot> RespondAsync(TacticalPlanResponse response,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastResponse = response;
            ResponseCount++;
            return ValueTask.FromResult(new TacticalExecutionSnapshot(_executionId, _intentId,
                TacticalExecutionState.Executing, "Fake execution started."));
        }

        public ValueTask<TacticalExecutionSnapshot> CancelAsync(Guid executionId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (executionId != _executionId) throw new ArgumentException("Unknown execution.", nameof(executionId));
            return ValueTask.FromResult(new TacticalExecutionSnapshot(_executionId, _intentId,
                TacticalExecutionState.Cancelled, "Fake execution cancelled.", 1));
        }
    }
}
