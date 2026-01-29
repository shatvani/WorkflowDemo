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

        services.AddSingleton<IProcessDefinitionStore, YamlProcessDefinitionStore>();  //FakeProcessDefinitionStore

        services.AddSingleton<WorkflowEngine>();

        return services;
    }

    public static IApplicationBuilder UseWorkflowModule(this IApplicationBuilder app) => app;

    public static IEndpointRouteBuilder MapWorkflowEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
