using Microsoft.Azure.Functions.Worker.ApplicationInsights;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using api.Storage;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Cosmos DB Configuration
var cosmosEndpoint = builder.Configuration["COSMOS_DB_ENDPOINT"];
var cosmosKey = builder.Configuration["COSMOS_DB_KEY"];
var cosmosDatabase = builder.Configuration["COSMOS_DB_DATABASE"];
var cosmosContainer = builder.Configuration["COSMOS_DB_CONTAINER"];

if (string.IsNullOrWhiteSpace(cosmosEndpoint) ||
    string.IsNullOrWhiteSpace(cosmosKey) ||
    string.IsNullOrWhiteSpace(cosmosDatabase) ||
    string.IsNullOrWhiteSpace(cosmosContainer))
{
    throw new InvalidOperationException("Cosmos DB configuration is missing.");
}

// Register the Cosmos-backed repository
builder.Services.AddSingleton<IUserRepository>(sp =>
{
    var clientOptions = new CosmosClientOptions
    {
        ConnectionMode = ConnectionMode.Gateway
    };

    var client = new CosmosClient(cosmosEndpoint, cosmosKey, clientOptions);

    var dbResponse = client.CreateDatabaseIfNotExistsAsync(cosmosDatabase).GetAwaiter().GetResult();
    var database = dbResponse.Database;

    var containerProps = new ContainerProperties(cosmosContainer, "/id");
    var containerResponse = database.CreateContainerIfNotExistsAsync(containerProps).GetAwaiter().GetResult();
    var container = containerResponse.Container;

    Console.WriteLine($"DEBUG: Cosmos DB container '{cosmosContainer}' ready.");

    return new CosmosUserRepository(container);
});

// BOOKS container
builder.Services.AddSingleton<IBookRepository>(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var db = client.GetDatabase(cosmosDatabase);

    var container = db.CreateContainerIfNotExistsAsync(
        new ContainerProperties("Books", "/OwnerUsername")
    ).GetAwaiter().GetResult().Container;

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

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
