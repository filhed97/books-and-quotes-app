using Microsoft.Azure.Functions.Worker.ApplicationInsights;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using api.Storage;

var builder = FunctionsApplication.CreateBuilder(args);

// Configure Application Insights
builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Cosmos DB configuration
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

// Register IUserRepository with Cosmos DB implementation
builder.Services.AddSingleton<IUserRepository>(sp =>
{
    // Configure client to use Gateway mode (emulator-friendly)
    var clientOptions = new CosmosClientOptions
    {
        ConnectionMode = ConnectionMode.Gateway
    };
    var client = new CosmosClient(cosmosEndpoint, cosmosKey, clientOptions);

    // Ensure database exists
    var databaseResponse = client.CreateDatabaseIfNotExistsAsync(cosmosDatabase).GetAwaiter().GetResult();
    var database = databaseResponse.Database;

    // Ensure container exists with partition key "/id"
    var containerProperties = new ContainerProperties(cosmosContainer, "/id");
    var containerResponse = database.CreateContainerIfNotExistsAsync(containerProperties).GetAwaiter().GetResult();
    var container = containerResponse.Container;

    Console.WriteLine($"DEBUG: Cosmos DB container '{cosmosContainer}' ready with partition key {containerProperties.PartitionKeyPath}");

    return new CosmosUserRepository(container);
});

// Configure Functions Web Application
builder.ConfigureFunctionsWebApplication();

// Build and run
builder.Build().Run();
