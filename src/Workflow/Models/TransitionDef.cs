namespace Workflow.Models;

public sealed class TransitionDef
{
    public required string From { get; init; }
    public required string On { get; init; }
    public required string To { get; init; }

    public bool IsEnd => To.StartsWith("end:", StringComparison.OrdinalIgnoreCase);
    public string EndKey => IsEnd ? To["end:".Length..] : "";
}
