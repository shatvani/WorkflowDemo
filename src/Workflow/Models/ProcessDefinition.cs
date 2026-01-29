using Workflow.Runtime;

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

    public string ResolveNextByOutcome(string fromNodeId, string outcome)
    {
        var next = Transitions.FirstOrDefault(t =>
            string.Equals(t.From, fromNodeId, StringComparison.OrdinalIgnoreCase) &&
            t.On is not null &&
            string.Equals(t.On, outcome, StringComparison.OrdinalIgnoreCase));

        if (next is null)
            throw new InvalidOperationException(
                $"No transition from '{fromNodeId}' on outcome '{outcome}' in process '{Id}' v{Version}.");

        return next.To;
    }

    public string ResolveNextByWhen(string fromNodeId, IReadOnlyDictionary<string, bool> variables)
    {
        var candidates = Transitions.Where(t =>
            string.Equals(t.From, fromNodeId, StringComparison.OrdinalIgnoreCase) &&
            t.When is not null).ToList();

        if (candidates.Count == 0)
            throw new InvalidOperationException(
                $"No 'when' transitions from '{fromNodeId}' in process '{Id}' v{Version}.");

        foreach (var t in candidates)
        {
            if (BooleanExpressionEvaluator.Eval(t.When!, variables))
                return t.To;
        }

        throw new InvalidOperationException(
            $"No 'when' condition matched from '{fromNodeId}' in process '{Id}' v{Version}'.");
    }
}
