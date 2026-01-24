var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWorkflowModule(builder.Configuration)
    .AddKozterModule(builder.Configuration); 

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseWorkflowModule()
   .UseKozterModule();

app.Run();
