using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Workflow.Runtime;

namespace Workflow.Endpoints;

public record SignalCaseRequest(Dictionary<string, bool> Variables);

public class SignalCaseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/workflow/cases/{caseId:guid}/signal",
        async (Guid caseId, SignalCaseRequest req, WorkflowEngine engine) =>
        {
            var result = await engine.Signal(caseId, req.Variables);
            return Results.Ok(result);
        })
        .WithName("SignalCase")
        .Produces<CompleteTaskResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
