using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kozter;

public static class KozterModule
{
    public static IServiceCollection AddKozterModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all validators from this assembly (FluentValidation core / DI extensions)
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        // Add services to the container.

        // Api Endpoint services

        // Application Use Case services


        // Data - Infrastructure services

        return services;
    }

    public static IApplicationBuilder UseKozterModule(this IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.

        // 1. Use Api Endpoint services

        // 2. Use Application Use Case services

        // 3. Use Data - Infrastructure services

        return app;
    }
}

