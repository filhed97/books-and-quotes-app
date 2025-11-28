namespace api.Storage;

using Azure;
using Azure.Data.Tables;

public class TableUserRepository : IUserRepository
{
    private readonly TableClient _table;

    public TableUserRepository(string connectionString)
    {
        var _client = new TableServiceClient(connectionString);
        _table = _client.GetTableClient("Users");
        _table.CreateIfNotExists();
    }

    public async Task CreateUserAsync(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("username is required", nameof(username));

        var entity = new TableUserEntity
        {
            RowKey = username,
            PasswordHash = passwordHash
        };

        // using Upsert to avoid duplicate errors during development
        await _table.UpsertEntityAsync(entity);
        //await _table.AddEntityAsync(entity);
    }

    public async Task<(string Username, string PasswordHash)?> GetUserAsync(string username)
    {
        try
        {
            var entity = await _table.GetEntityAsync<TableUserEntity>("USER", username);
            return (entity.Value.RowKey, entity.Value.PasswordHash);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }
}
