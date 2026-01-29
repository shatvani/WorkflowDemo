using Workflow.Models;
using Workflow.Storage;

namespace Workflow.Runtime;

public sealed class WorkflowEngine
{
    private readonly IProcessDefinitionStore _defs;
    private readonly InMemoryCaseStore _store;

    public WorkflowEngine(IProcessDefinitionStore defs, InMemoryCaseStore store)
    {
        _defs = defs;
        _store = store;
    }

    public async Task<StartCaseResponse> StartCase(string processId, int version, CancellationToken ct = default)
    {
        var def = await _defs.Load(processId, version, ct);

        var startStep = def.GetStep(def.StartStepId);

        var c = new CaseInstance
        {
            ProcessId = def.Id,
            Version = def.Version,
            CurrentNodeId = startStep.Id,
            Status = CaseStatus.Active
        };
        _store.AddCase(c);

        var task = new TaskInstance
        {
            CaseId = c.Id,
            StepId = startStep.Id,
            TaskType = startStep.TaskType,
            Status = TaskStatus.Open
        };
        _store.AddTask(task);

        return new StartCaseResponse(c.Id, task.Id, task.TaskType, task.StepId);
    }

    public async Task<CompleteTaskResponse> CompleteTask(Guid caseId, Guid taskId, string outcome, CancellationToken ct = default)
    {
        var c = _store.GetCase(caseId);
        if (c.Status == CaseStatus.Finished)
            throw new InvalidOperationException($"Case '{caseId}' is finished.");

        var task = _store.GetTask(taskId);
        if (task.CaseId != caseId)
            throw new InvalidOperationException("Task does not belong to this case.");
        if (task.Status != TaskStatus.Open)
            throw new InvalidOperationException("Task is not open.");

        var def = await _defs.Load(c.ProcessId, c.Version, ct);

        // outcome validálás: csak step node-on
        var step = def.GetStep(task.StepId);
        if (!step.Outcomes.Contains(outcome))
            throw new InvalidOperationException($"Outcome '{outcome}' is not allowed for step '{step.Id}'.");

        // task lezárás
        task.Status = TaskStatus.Completed;
        task.Outcome = outcome;
        _store.UpdateTask(task);

        // következő node
        var next = def.ResolveNextByOutcome(step.Id, outcome);

        // léptetés guard/wait/end figyelembevételével
        return Advance(def, c, next);
    }

    public async Task<CompleteTaskResponse> Signal(Guid caseId, Dictionary<string, bool> variables, CancellationToken ct = default)
    {
        var c = _store.GetCase(caseId);
        if (c.Status != CaseStatus.Waiting)
            throw new InvalidOperationException($"Case '{caseId}' is not in Waiting state.");

        // merge variables
        foreach (var kv in variables)
            c.Variables[kv.Key] = kv.Value;

        var def = await _defs.Load(c.ProcessId, c.Version, ct);

        // wait node-ról when alapján tovább
        var next = def.ResolveNextByWhen(c.CurrentNodeId, c.Variables);
        return Advance(def, c, next);
    }

    private CompleteTaskResponse Advance(ProcessDefinition def, CaseInstance c, string nextNodeId)
    {
        while (true)
        {
            // end
            if (nextNodeId.StartsWith("end:", StringComparison.OrdinalIgnoreCase))
            {
                c.Status = CaseStatus.Finished;
                c.CurrentNodeId = nextNodeId;
                _store.UpdateCase(c);

                return new CompleteTaskResponse(
                    CaseId: c.Id,
                    CaseStatus: c.Status.ToString(),
                    IsFinished: true,
                    NextTaskId: null,
                    NextTaskType: null,
                    NextStepId: null
                );
            }

            // wait
            if (nextNodeId.StartsWith("wait:", StringComparison.OrdinalIgnoreCase))
            {
                c.Status = CaseStatus.Waiting;
                c.CurrentNodeId = nextNodeId;
                _store.UpdateCase(c);

                return new CompleteTaskResponse(
                    CaseId: c.Id,
                    CaseStatus: c.Status.ToString(),
                    IsFinished: false,
                    NextTaskId: null,
                    NextTaskType: null,
                    NextStepId: null
                );
            }

            // guard
            if (nextNodeId.StartsWith("guard:", StringComparison.OrdinalIgnoreCase))
            {
                c.CurrentNodeId = nextNodeId;
                _store.UpdateCase(c);

                nextNodeId = def.ResolveNextByWhen(nextNodeId, c.Variables);
                continue;
            }

            // step
            var nextStep = def.GetStep(nextNodeId);
            c.Status = CaseStatus.Active;
            c.CurrentNodeId = nextStep.Id;
            _store.UpdateCase(c);

            var nextTask = new TaskInstance
            {
                CaseId = c.Id,
                StepId = nextStep.Id,
                TaskType = nextStep.TaskType,
                Status = TaskStatus.Open
            };
            _store.AddTask(nextTask);

            return new CompleteTaskResponse(
                CaseId: c.Id,
                CaseStatus: c.Status.ToString(),
                IsFinished: false,
                NextTaskId: nextTask.Id,
                NextTaskType: nextTask.TaskType,
                NextStepId: nextTask.StepId
            );
        }
    }
}
