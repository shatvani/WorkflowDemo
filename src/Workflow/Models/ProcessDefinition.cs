namespace Workflow.Models;

public sealed class ProcessDefinition
{
    public required string Id { get; init; }
    public required int Version { get; init; }
    public required string StartStepId { get; init; }

    public required Dictionary<string, StepDef> Steps { get; init; } = new();
    public required List<TransitionDef> Transitions { get; init; } = new();

    public StepDef GetStep(string stepId)
        => Steps.TryGetValue(stepId, out var step)
            ? step
            : throw new InvalidOperationException($"Unknown step '{stepId}' in process '{Id}' v{Version}.");

    public string ResolveNext(string fromStepId, string outcome)
    {
        var next = Transitions.FirstOrDefault(t => t.From == fromStepId && t.On == outcome);
        if (next is null)
            throw new InvalidOperationException($"No transition from '{fromStepId}' on outcome '{outcome}' in process '{Id}' v{Version}.");

        return next.To;
    }
}
