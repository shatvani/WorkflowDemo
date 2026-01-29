namespace Workflow.Runtime;

public enum CaseStatus
{
    Active,
    Waiting,
    Finished
}

public sealed class CaseInstance
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string ProcessId { get; init; }
    public required int Version { get; init; }

    public CaseStatus Status { get; set; } = CaseStatus.Active;

    // lehet step.*, guard:*, wait:*
    public required string CurrentNodeId { get; set; }

    // guard/wait döntésekhez
    public Dictionary<string, bool> Variables { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
