namespace Workflow.Runtime;

public record StartCaseResponse(Guid CaseId, Guid TaskId, string TaskType, string StepId);

public record CompleteTaskResponse(
    Guid CaseId,
    string CaseStatus,
    bool IsFinished,
    Guid? NextTaskId,
    string? NextTaskType,
    string? NextStepId
);
