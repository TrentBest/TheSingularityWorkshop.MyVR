namespace TheSingularityWorkshop.MyVR.Tactics;

/// <summary>
/// Coordinates one player intent through bot proposal, player decision, and execution.
/// The workflow records provenance but never trains or changes the configured bot.
/// </summary>
public sealed class TacticalCommandWorkflow
{
    private readonly ITacticalCommandAdapter _adapter;
    private readonly Func<DateTimeOffset> _utcNow;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly List<TacticalLearningEvent> _events = new();

    private TacticalIntent? _intent;
    private TacticalPlanProposal? _proposal;
    private TacticalPlanDecision? _decision;
    private TacticalExecutionSnapshot? _execution;

    public TacticalCommandWorkflow(
        ITacticalCommandAdapter adapter,
        Func<DateTimeOffset>? utcNow = null)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        _adapter = adapter;
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
    }

    /// <summary>Submits a single player intent and obtains a validated bot proposal.</summary>
    public async ValueTask<TacticalWorkflowSnapshot> SubmitIntentAsync(
        TacticalIntent intent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(intent);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_intent is not null)
                throw new InvalidOperationException("This workflow already has an intent.");

            var proposal = await _adapter.ProposeAsync(intent, cancellationToken).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(proposal);
            proposal.Validate();
            if (proposal.IntentId != intent.IntentId)
                throw new InvalidOperationException("The bot proposal does not correlate to the submitted intent.");

            _intent = intent;
            _proposal = proposal;
            Record(intent.IntentId, "intent.submitted", "player", intent.Kind.ToString());
            Record(intent.IntentId, "plan.proposed", "bot", proposal.Summary);
            return CreateSnapshot();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Applies the player's approve, modify, or reject decision to the current proposal.</summary>
    public async ValueTask<TacticalWorkflowSnapshot> RespondAsync(
        TacticalPlanResponse response,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_intent is null || _proposal is null)
                throw new InvalidOperationException("Submit an intent before responding to a plan.");
            if (_decision is not null)
                throw new InvalidOperationException("The current proposal already has a player decision.");
            if (response.ProposalId != _proposal.ProposalId)
                throw new ArgumentException("The response does not identify the current proposal.", nameof(response));
            if (!Enum.IsDefined(response.Decision))
                throw new ArgumentOutOfRangeException(nameof(response));

            if (response.Decision == TacticalPlanDecision.Modify)
                ValidateModifiedGroups(response.ModifiedGroups);
            else if (response.ModifiedGroups is not null)
                throw new ArgumentException("Modified groups are only valid with a Modify decision.", nameof(response));

            _decision = response.Decision;
            var detail = response.Decision == TacticalPlanDecision.Modify
                ? $"Player supplied {response.ModifiedGroups!.Count} replacement groups."
                : response.Decision.ToString();
            Record(_intent.IntentId, $"plan.{response.Decision.ToString().ToLowerInvariant()}", "player", detail);

            if (response.Decision != TacticalPlanDecision.Reject)
            {
                var execution = await _adapter.RespondAsync(response, cancellationToken).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(execution);
                if (execution.IntentId != _intent.IntentId || execution.ExecutionId == Guid.Empty)
                    throw new InvalidOperationException("The adapter returned an execution snapshot with invalid correlation.");
                if (execution.State is not (TacticalExecutionState.Accepted or TacticalExecutionState.Staging
                    or TacticalExecutionState.Ready or TacticalExecutionState.Executing))
                    throw new InvalidOperationException("The adapter returned a non-active state for an accepted plan.");

                _execution = execution;
                RecordExecution(execution);
            }

            return CreateSnapshot();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Cancels an active execution and records the adapter-confirmed terminal state.</summary>
    public async ValueTask<TacticalWorkflowSnapshot> CancelAsync(
        CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_intent is null || _execution is null)
                throw new InvalidOperationException("There is no accepted execution to cancel.");
            if (IsTerminal(_execution.State))
                throw new InvalidOperationException("A terminal execution cannot be cancelled.");

            var cancelled = await _adapter.CancelAsync(_execution.ExecutionId, cancellationToken)
                .ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(cancelled);
            if (cancelled.ExecutionId != _execution.ExecutionId
                || cancelled.IntentId != _intent.IntentId
                || cancelled.State != TacticalExecutionState.Cancelled)
                throw new InvalidOperationException("The adapter did not confirm cancellation of the active execution.");

            _execution = cancelled;
            Record(_intent.IntentId, "execution.cancelled", "adapter", cancelled.Message);
            return CreateSnapshot();
        }
        finally
        {
            _gate.Release();
        }
    }

    private void RecordExecution(TacticalExecutionSnapshot execution) =>
        Record(execution.IntentId, $"execution.{execution.State.ToString().ToLowerInvariant()}",
            "adapter", execution.Message);

    private void Record(Guid intentId, string eventType, string actor, string? details) =>
        _events.Add(new TacticalLearningEvent(Guid.NewGuid(), intentId, eventType, _utcNow(), actor, details));

    private TacticalWorkflowSnapshot CreateSnapshot() =>
        new(_intent!, _proposal!, _decision, _execution, _events.ToArray());

    private static bool IsTerminal(TacticalExecutionState state) =>
        state is TacticalExecutionState.Completed or TacticalExecutionState.Failed
            or TacticalExecutionState.Cancelled or TacticalExecutionState.Degraded;

    private static void ValidateModifiedGroups(IReadOnlyList<TacticalGroupPlan>? groups)
    {
        ArgumentNullException.ThrowIfNull(groups);
        if (groups.Count == 0)
            throw new ArgumentException("A modified plan must contain at least one group.", nameof(groups));
        if (groups.Select(group => group.GroupId).Distinct(StringComparer.Ordinal).Count() != groups.Count)
            throw new ArgumentException("Modified group IDs must be unique.", nameof(groups));
    }
}

/// <summary>Immutable view of the workflow and its attributable decision trail.</summary>
public sealed record TacticalWorkflowSnapshot(
    TacticalIntent Intent,
    TacticalPlanProposal Proposal,
    TacticalPlanDecision? Decision,
    TacticalExecutionSnapshot? Execution,
    IReadOnlyList<TacticalLearningEvent> LearningEvents);
