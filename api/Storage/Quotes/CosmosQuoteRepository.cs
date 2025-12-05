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

    public async Task<Quote?> GetAsync(string id, string owner)
    {
        try
        {
            var resp = await _container.ReadItemAsync<Quote>(id, new PartitionKey(owner));
            return resp.Resource;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Quote>> ListAsync(string owner)
    {
        var q = _container.GetItemLinqQueryable<Quote>(true)
                          .Where(qt => qt.userId == owner)
                          .ToFeedIterator();

        var results = new List<Quote>();

        while (q.HasMoreResults)
        {
            results.AddRange(await q.ReadNextAsync());
        }

        return results;
    }

    public Task AddAsync(Quote quote) =>
        _container.CreateItemAsync(quote, new PartitionKey(quote.userId));

    public Task UpdateAsync(Quote quote) =>
        _container.UpsertItemAsync(quote, new PartitionKey(quote.userId));

    public Task DeleteAsync(string id, string owner) =>
        _container.DeleteItemAsync<Quote>(id, new PartitionKey(owner));
}
