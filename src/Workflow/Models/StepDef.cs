namespace Workflow.Models;

public sealed class StepDef
{
    public required string Id { get; init; }
    public required string TaskType { get; init; }
    public required HashSet<string> Outcomes { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
