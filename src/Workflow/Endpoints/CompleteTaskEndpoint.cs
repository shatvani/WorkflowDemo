using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Workflow.Runtime;

namespace Workflow.Endpoints;

public record CompleteTaskRequest(string Outcome);

public record CompleteTaskResponse(
    Guid CaseId,
    string CaseStatus,
    bool IsFinished,
    Guid? NextTaskId,
    string? NextTaskType,
    string? NextStepId
);

public class CompleteTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/workflow/cases/{caseId:guid}/tasks/{taskId:guid}/complete",
        async (Guid caseId, Guid taskId, CompleteTaskRequest req, WorkflowEngine engine) =>
        {
            var result = await engine.CompleteTask(caseId, taskId, req.Outcome);
            return Results.Ok(result);
        })
        .WithName("CompleteTask")
        .Produces<CompleteTaskResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Complete task")
        .WithDescription("Complete task");
    }
}