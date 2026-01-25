using Workflow.Endpoints;
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
            CurrentStepId = startStep.Id,
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
        if (c.Status != CaseStatus.Active)
            throw new InvalidOperationException($"Case '{caseId}' is not active.");

        var task = _store.GetTask(taskId);
        if (task.CaseId != caseId)
            throw new InvalidOperationException("Task does not belong to this case.");
        if (task.Status != TaskStatus.Open)
            throw new InvalidOperationException("Task is not open.");

        var def = await _defs.Load(c.ProcessId, c.Version, ct);
        var step = def.GetStep(task.StepId);

        // outcome validálás (MVP)
        if (!step.Outcomes.Contains(outcome))
            throw new InvalidOperationException($"Outcome '{outcome}' is not allowed for step '{step.Id}'.");

        // task lezárás
        task.Status = TaskStatus.Completed;
        task.Outcome = outcome;
        _store.UpdateTask(task);

        // következő lépés
        var next = def.ResolveNext(step.Id, outcome);

        // end?
        if (next.StartsWith("end:", StringComparison.OrdinalIgnoreCase))
        {
            c.Status = CaseStatus.Finished;
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

        // next step task létrehozás
        var nextStep = def.GetStep(next);
        c.CurrentStepId = nextStep.Id;
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
