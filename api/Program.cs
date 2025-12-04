using Microsoft.Azure.Functions.Worker.ApplicationInsights;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using api.Storage;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Load Cosmos DB configuration
var cosmosEndpoint = builder.Configuration["COSMOS_DB_ENDPOINT"];
var cosmosKey = builder.Configuration["COSMOS_DB_KEY"];
var databaseName = builder.Configuration["COSMOS_DB_DATABASE"];

if (string.IsNullOrWhiteSpace(cosmosEndpoint) ||
    string.IsNullOrWhiteSpace(cosmosKey) ||
    string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException("Cosmos DB configuration is missing.");
}

// Register a singleton CosmosClient
builder.Services.AddSingleton(sp =>
{
    return new CosmosClient(
        cosmosEndpoint,
        cosmosKey,
        new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
        }
    );
});

// Get a reference to the existing database
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    return client.GetDatabase(databaseName);
});

// Get references to the existing containers (no creation)
builder.Services.AddSingleton<IUserRepository>(sp =>
{
    var db = sp.GetRequiredService<Database>();
    var container = db.GetContainer("Users");
    return new CosmosUserRepository(container);
});

builder.Services.AddSingleton<IBookRepository>(sp =>
{
    var db = sp.GetRequiredService<Database>();
    var container = db.GetContainer("Books");
    Console.WriteLine("DEBUG: Books container reference ready.");
    return new CosmosBookRepository(container);
});

// JWT configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var jwtKey = builder.Configuration["JWT_KEY"];
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "books-and-quotes-api";

if (string.IsNullOrWhiteSpace(jwtKey))
{
    Console.WriteLine("WARNING: JWT_KEY is missing from config.");
}

// Build and run the Functions app
builder.ConfigureFunctionsWebApplication();
builder.Build().Run();
