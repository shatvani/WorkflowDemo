namespace Workflow.Runtime;

public enum TaskStatus { Open, Completed }

public sealed class TaskInstance
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid CaseId { get; init; }
    public required string StepId { get; init; }
    public required string TaskType { get; init; }
    public TaskStatus Status { get; set; } = TaskStatus.Open;
    // amit a feladat befejezésekor mondasz (pl. ok, missing_info_needed, suspend_requested)
    public string? Outcome { get; set; }
}
