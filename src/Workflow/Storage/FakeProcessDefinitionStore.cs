using Workflow.Models;

namespace Workflow.Storage;

public sealed class FakeProcessDefinitionStore : IProcessDefinitionStore
{
    public Task<ProcessDefinition> Load(string processId, int version, CancellationToken ct = default)
    {
        // MVP: csak egy demo process-t tudunk
        if (processId != "demo_kozter_mini" || version != 1)
            throw new InvalidOperationException($"Unknown process '{processId}' v{version}.");

        var def = new ProcessDefinition
        {
            Id = "demo_kozter_mini",
            Version = 1,
            StartStepId = "step.capture",
            Steps = new Dictionary<string, StepDef>
            {
                ["step.capture"] = new StepDef
                {
                    Id = "step.capture",
                    TaskType = "01",
                    Outcomes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ok" }
                },
                ["step.review"] = new StepDef
                {
                    Id = "step.review",
                    TaskType = "08",
                    Outcomes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ok", "missing_info_needed" }
                },
                ["step.missing_info"] = new StepDef
                {
                    Id = "step.missing_info",
                    TaskType = "13",
                    Outcomes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "provided" }
                },
                ["step.dispatch"] = new StepDef
                {
                    Id = "step.dispatch",
                    TaskType = "27",
                    Outcomes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "dispatched" }
                }
            },
            Transitions = new List<TransitionDef>
            {
                new() { From = "step.capture", On = "ok", To = "step.review" },

                new() { From = "step.review", On = "missing_info_needed", To = "step.missing_info" },
                new() { From = "step.review", On = "ok", To = "step.dispatch" },

                new() { From = "step.missing_info", On = "provided", To = "step.review" },

                new() { From = "step.dispatch", On = "dispatched", To = "end:done" }
            }
        };

        return Task.FromResult(def);
    }
}
