using Microsoft.Azure.Cosmos;

namespace api.Storage;
public class CosmosUserRepository : IUserRepository
{
    private readonly Container _container;

    public CosmosUserRepository(Container container)
    {
        _container = container;
    }

    public async Task CreateUserAsync(string username, string passwordHash)
    {
        // Construct a proper object with Id set for partition key
        var user = new User
        {
            Id = username, 
            Username = username,
            PasswordHash = passwordHash
        };

        Console.WriteLine($"DEBUG: PartitionKey = '{new PartitionKey(user.Username).ToString()}'");

        var json = System.Text.Json.JsonSerializer.Serialize(user);
        Console.WriteLine($"DEBUG JSON: {json}");

        await _container.CreateItemAsync(user, new PartitionKey(user.Username));
    }

    public async Task<(string Username, string PasswordHash)?> GetUserAsync(string username)
    {
        try
        {
            var response = await _container.ReadItemAsync<User>(username, new PartitionKey(username));
            var u = response.Resource;
            return (u.Username, u.PasswordHash);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
