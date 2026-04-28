using SupportOpsAI.Infrastructure;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<ICurrentUserService, WorkerCurrentUserService>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddTriageWorker();

var host = builder.Build();
host.Run();
