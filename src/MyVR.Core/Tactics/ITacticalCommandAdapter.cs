namespace TheSingularityWorkshop.MyVR.Tactics;

/// <summary>
/// Protocol-independent boundary to the configured bot. Implementations own game rules,
/// unit selection, composition, pathing, timing, and translation into game actions.
/// </summary>
public interface ITacticalCommandAdapter
{
    ValueTask<TacticalPlanProposal> ProposeAsync(
        TacticalIntent intent,
        CancellationToken cancellationToken = default);

    ValueTask<TacticalExecutionSnapshot> RespondAsync(
        TacticalPlanResponse response,
        CancellationToken cancellationToken = default);

    ValueTask<TacticalExecutionSnapshot> CancelAsync(
        Guid executionId,
        CancellationToken cancellationToken = default);
}
