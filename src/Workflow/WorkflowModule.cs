using Microsoft.AspNetCore.Builder;              // IApplicationBuilder - Microsoft.AspNetCore
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;        // IConfiguration
using Microsoft.Extensions.DependencyInjection;
using Workflow.Runtime;
using Workflow.Storage;  // IServiceCollection

namespace Workflow;

public static class WorkflowModule
{
    public static IServiceCollection AddWorkflowModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InMemoryCaseStore>();

        services.AddSingleton<IProcessDefinitionStore, FakeProcessDefinitionStore>();

        services.AddSingleton<WorkflowEngine>();

        return services;
    }

    public static IApplicationBuilder UseWorkflowModule(this IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.

        // 1. Use Api Endpoint services

        // 2. Use Application Use Case services

        // 3. Use Data - Infrastructure services

        return app;
    }

    public static IEndpointRouteBuilder MapWorkflowEndpoints(this IEndpointRouteBuilder endpoints)
    {
               // Map Workflow related endpoints here
        return endpoints;
    }
}
