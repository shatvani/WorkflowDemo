using Workflow.Runtime;
using TaskStatus = Workflow.Runtime.TaskStatus;

namespace Workflow.Storage;

public sealed class InMemoryCaseStore
{
    private readonly Dictionary<Guid, CaseInstance> _cases = new();
    private readonly Dictionary<Guid, TaskInstance> _tasks = new();

    public CaseInstance AddCase(CaseInstance c)
    {
        _cases[c.Id] = c;
        return c;
    }

    public TaskInstance AddTask(TaskInstance t)
    {
        _tasks[t.Id] = t;
        return t;
    }

    public CaseInstance GetCase(Guid caseId)
        => _cases.TryGetValue(caseId, out var c) ? c : throw new InvalidOperationException($"Case '{caseId}' not found.");

    public TaskInstance GetTask(Guid taskId)
        => _tasks.TryGetValue(taskId, out var t) ? t : throw new InvalidOperationException($"Task '{taskId}' not found.");

    public IEnumerable<TaskInstance> GetOpenTasks(Guid caseId)
        => _tasks.Values.Where(t => t.CaseId == caseId && t.Status == TaskStatus.Open);

    public void UpdateTask(TaskInstance t) => _tasks[t.Id] = t;
    public void UpdateCase(CaseInstance c) => _cases[c.Id] = c;

    public IEnumerable<TaskInstance> GetAllTasks(Guid caseId)
    => _tasks.Values.Where(t => t.CaseId == caseId)
                    .OrderBy(t => t.Status)
                    .ThenBy(t => t.StepId);

    public bool TryGetCase(Guid caseId, out CaseInstance? c)
    => _cases.TryGetValue(caseId, out c);

    public bool TryGetTask(Guid taskId, out TaskInstance? t)
        => _tasks.TryGetValue(taskId, out t);
}
