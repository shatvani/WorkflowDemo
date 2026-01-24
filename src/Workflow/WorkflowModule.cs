using Microsoft.AspNetCore.Builder;              // IApplicationBuilder - Microsoft.AspNetCore
using Microsoft.Extensions.Configuration;        // IConfiguration
using Microsoft.Extensions.DependencyInjection;  // IServiceCollection

namespace Workflow;

public static class WorkflowModule
{
    public static IServiceCollection AddWorkflowModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all validators from this assembly (FluentValidation core / DI extensions)
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        // Add services to the container.

        // Api Endpoint services

        // Application Use Case services


        // Data - Infrastructure services

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
}
