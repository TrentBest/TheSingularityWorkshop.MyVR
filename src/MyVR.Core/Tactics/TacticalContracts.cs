namespace TheSingularityWorkshop.MyVR.Tactics;

/// <summary>Player-level objective. Values are semantic and contain no game-protocol concepts.</summary>
public enum TacticalIntentKind
{
    FlankLeft,
    FlankRight,
    SplitIntoGroups,
    AttackPosition,
    SynchronizedAttack,
    Hold,
    Regroup,
    FallBack
}

/// <summary>A spatial target selected by the client, expressed in a named coordinate frame.</summary>
public sealed record TacticalTarget
{
    public string TargetId { get; }
    public string CoordinateFrameId { get; }
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public TacticalTarget(string targetId, string coordinateFrameId, double x, double y, double z)
    {
        TargetId = Require(targetId, nameof(targetId));
        CoordinateFrameId = Require(coordinateFrameId, nameof(coordinateFrameId));
        if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(z))
            throw new ArgumentOutOfRangeException(nameof(x), "Target coordinates must be finite.");
        X = x;
        Y = y;
        Z = z;
    }

    private static string Require(string value, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, name);
        return value.Trim();
    }
}

/// <summary>Player intent, kept distinct from any bot-authored execution plan.</summary>
public sealed record TacticalIntent
{
    public Guid IntentId { get; }
    public TacticalIntentKind Kind { get; }
    public TacticalTarget? Target { get; }
    public int? RequestedGroupCount { get; }
    public DateTimeOffset IssuedAt { get; }

    public TacticalIntent(
        TacticalIntentKind kind,
        TacticalTarget? target = null,
        int? requestedGroupCount = null,
        Guid? intentId = null,
        DateTimeOffset? issuedAt = null)
    {
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        if (requestedGroupCount is <= 0) throw new ArgumentOutOfRangeException(nameof(requestedGroupCount));
        if (kind is TacticalIntentKind.AttackPosition or TacticalIntentKind.SynchronizedAttack && target is null)
            throw new ArgumentException("This intent requires a spatial target.", nameof(target));
        if (kind == TacticalIntentKind.SplitIntoGroups && requestedGroupCount is null)
            throw new ArgumentException("Splitting requires a requested group count.", nameof(requestedGroupCount));

        IntentId = intentId ?? Guid.NewGuid();
        if (IntentId == Guid.Empty) throw new ArgumentException("Intent ID must not be empty.", nameof(intentId));
        Kind = kind;
        Target = target;
        RequestedGroupCount = requestedGroupCount;
        IssuedAt = issuedAt ?? DateTimeOffset.UtcNow;
    }
}

/// <summary>Bot-proposed group role and route summary; game-specific units remain bot-owned.</summary>
public sealed record TacticalGroupPlan(string GroupId, string Role, string RouteSummary)
{
    public string GroupId { get; } = Require(GroupId, nameof(GroupId));
    public string Role { get; } = Require(Role, nameof(Role));
    public string RouteSummary { get; } = Require(RouteSummary, nameof(RouteSummary));

    private static string Require(string value, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, name);
        return value.Trim();
    }
}

/// <summary>Explicit synchronization semantics for coordinated multi-group execution.</summary>
public sealed record TacticalSynchronization
{
    public TacticalSynchronizationKind Kind { get; }
    public TimeSpan ArrivalTolerance { get; }

    public TacticalSynchronization(TacticalSynchronizationKind kind, TimeSpan arrivalTolerance)
    {
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        if (arrivalTolerance < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(arrivalTolerance));
        Kind = kind;
        ArrivalTolerance = arrivalTolerance;
    }
}

public enum TacticalSynchronizationKind
{
    None,
    SharedRelease,
    CoordinatedArrival
}

/// <summary>A bot's proposed plan. It is not executable until the player approves it.</summary>
public sealed record TacticalPlanProposal(
    Guid ProposalId,
    Guid IntentId,
    string Summary,
    IReadOnlyList<TacticalGroupPlan> Groups,
    TacticalSynchronization? Synchronization = null)
{
    public void Validate()
    {
        if (ProposalId == Guid.Empty) throw new InvalidOperationException("Proposal ID must not be empty.");
        if (IntentId == Guid.Empty) throw new InvalidOperationException("Intent ID must not be empty.");
        ArgumentException.ThrowIfNullOrWhiteSpace(Summary);
        ArgumentNullException.ThrowIfNull(Groups);
        if (Groups.Count == 0) throw new InvalidOperationException("A proposal must contain at least one group.");
        if (Groups.Select(group => group.GroupId).Distinct(StringComparer.Ordinal).Count() != Groups.Count)
            throw new InvalidOperationException("Group IDs must be unique within a proposal.");
    }
}

public enum TacticalPlanDecision
{
    Approve,
    Modify,
    Reject
}

/// <summary>Player decision and optional replacement proposal content.</summary>
public sealed record TacticalPlanResponse(
    Guid ProposalId,
    TacticalPlanDecision Decision,
    IReadOnlyList<TacticalGroupPlan>? ModifiedGroups = null);

public enum TacticalExecutionState
{
    Accepted,
    Staging,
    Ready,
    Executing,
    Completed,
    Failed,
    Cancelled,
    Degraded
}

/// <summary>
/// A revisioned view of one bot-owned execution. Revisions increase monotonically for each update.
/// The initial execution snapshot starts at revision zero.
/// </summary>
public sealed record TacticalExecutionSnapshot(
    Guid ExecutionId,
    Guid IntentId,
    TacticalExecutionState State,
    string? Message = null,
    long Revision = 0);

/// <summary>Attributable event for future learning; this contract does not train or mutate a bot.</summary>
public sealed record TacticalLearningEvent(
    Guid EventId,
    Guid IntentId,
    string EventType,
    DateTimeOffset OccurredAt,
    string Actor,
    string? Details = null);
