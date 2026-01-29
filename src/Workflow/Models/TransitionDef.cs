namespace Workflow.Models;

public sealed class TransitionDef
{
    // honnan (step/guard/wait)
    public required string From { get; init; }
    // outcome esemény (csak step task completionből jön)
    public string? On { get; init; }
    // feltétel (guard/wait esetén)
    public string? When { get; init; }
    // hova (step/guard/wait/end)
    public required string To { get; init; }
    public string? Note { get; init; }

    public bool IsEnd => To.StartsWith("end:", StringComparison.OrdinalIgnoreCase);
    public bool IsWait => To.StartsWith("wait:", StringComparison.OrdinalIgnoreCase);
    public bool IsGuard => To.StartsWith("guard:", StringComparison.OrdinalIgnoreCase);

    public string EndKey => IsEnd ? To["end:".Length..] : "";
    public string WaitKey => IsWait ? To["wait:".Length..] : "";
    public string GuardKey => IsGuard ? To["guard:".Length..] : "";
}
