namespace Workflow.Runtime;

public enum CaseStatus { Active, Finished }

public sealed class CaseInstance
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string ProcessId { get; init; }
    public required int Version { get; init; }
    public CaseStatus Status { get; set; } = CaseStatus.Active;
    public required string CurrentStepId { get; set; }
}
