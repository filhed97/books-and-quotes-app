using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using api.Models;

namespace api.Storage;

public class CosmosQuoteRepository : IQuoteRepository
{
    private readonly Container _container;

    public CosmosQuoteRepository(Container container)
    {
        _container = container;
    }

    public async Task<Quote?> GetAsync(string id)
    {
        try
        {
            var resp = await _container.ReadItemAsync<Quote>(
                id,
                new PartitionKey("quotes")
            );
            return resp.Resource;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Quote>> ListAsync()
    {
        var query = _container.GetItemLinqQueryable<Quote>(true)
                              .Where(q => q.partitionKey == "quotes")
                              .ToFeedIterator();

        var results = new List<Quote>();

        while (query.HasMoreResults)
        {
            results.AddRange(await query.ReadNextAsync());
        }

        return results;
    }

    public Task AddAsync(Quote quote) =>
        _container.CreateItemAsync(quote, new PartitionKey("quotes"));

    public Task UpdateAsync(Quote quote) =>
        _container.UpsertItemAsync(quote, new PartitionKey("quotes"));

    public Task DeleteAsync(string id) =>
        _container.DeleteItemAsync<Quote>(id, new PartitionKey("quotes"));
}
