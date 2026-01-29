using Carter;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarterWithAssemblies(
    typeof(WorkflowModule).Assembly,
    typeof(KozterModule).Assembly
);

builder.Services
    .AddWorkflowModule(builder.Configuration)
    .AddKozterModule(builder.Configuration); 

var app = builder.Build();

app.MapCarter();

app.UseWorkflowModule()
    .UseKozterModule();

app.Run();
