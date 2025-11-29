using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using api.Storage;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var connectionString = builder.Configuration["TableStorageConnection"];

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("TableStorageConnection is missing.");

builder.Services.AddSingleton<IUserRepository>(
    new TableUserRepository(connectionString)
); 

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
