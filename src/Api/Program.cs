using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();

builder.Services
    .AddWorkflowModule(builder.Configuration)
    .AddKozterModule(builder.Configuration); 

var app = builder.Build();

app.MapCarter();

app.UseWorkflowModule()
    .UseKozterModule();
app.MapWorkflowEndpoints();

app.Run();
