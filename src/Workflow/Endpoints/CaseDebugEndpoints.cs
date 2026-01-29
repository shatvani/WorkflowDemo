using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Workflow.Storage;

namespace Workflow.Endpoints;

public class CaseDebugEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET /workflow/cases/{caseId}
        app.MapGet("/workflow/cases/{caseId:guid}",
        (Guid caseId, InMemoryCaseStore store) =>
        {
            if (!store.TryGetCase(caseId, out var c) || c is null)
                return Results.NotFound();

            var response = new CaseDetailsResponse(
                CaseId: c.Id,
                ProcessId: c.ProcessId,
                Version: c.Version,
                Status: c.Status.ToString(),
                CurrentNodeId: c.CurrentNodeId
            );

            return Results.Ok(response);
        })
        .WithName("GetCase")
        .Produces<CaseDetailsResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // GET /workflow/cases/{caseId}/tasks
        app.MapGet("/workflow/cases/{caseId:guid}/tasks",
        (Guid caseId, InMemoryCaseStore store) =>
        {
            if (!store.TryGetCase(caseId, out var c) || c is null)
                return Results.NotFound();

            var tasks = store.GetAllTasks(caseId)
                .Select(t => new TaskDetailsResponse(
                    TaskId: t.Id,
                    CaseId: t.CaseId,
                    StepId: t.StepId,
                    TaskType: t.TaskType,
                    Status: t.Status.ToString(),
                    Outcome: t.Outcome
                ))
                .ToList();

            return Results.Ok(tasks);
        })
        .WithName("GetCaseTasks")
        .Produces<List<TaskDetailsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public record CaseDetailsResponse(
        Guid CaseId,
        string ProcessId,
        int Version,
        string Status,
        string CurrentNodeId
    );

    public record TaskDetailsResponse(
        Guid TaskId,
        Guid CaseId,
        string StepId,
        string TaskType,
        string Status,
        string? Outcome
    );
}
