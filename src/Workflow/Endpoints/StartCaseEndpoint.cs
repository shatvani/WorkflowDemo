using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Workflow.Runtime;

namespace Workflow.Endpoints;

public record StartCaseRequest(string ProcessId, int Version);

public class StartCaseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/workflow/cases/start",
        async (StartCaseRequest req, WorkflowEngine engine) =>
        {
            var result = await engine.StartCase(req.ProcessId, req.Version);
            return Results.Created($"/workflow/cases/{result.CaseId}", result);
        })
        .WithName("StartCase")
        .Produces<StartCaseResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Start case")
        .WithDescription("Start case");
    }
}