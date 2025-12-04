using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using api.Models;

namespace api.Storage;

public class CosmosBookRepository : IBookRepository
{
    private readonly Container _container;

    public CosmosBookRepository(Container container)
    {
        _container = container;
    }

    public async Task<Book?> GetAsync(string id, string owner)
    {
        try
        {
            var resp = await _container.ReadItemAsync<Book>(id, new PartitionKey(owner));
            return resp.Resource;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Book>> ListAsync(string owner)
    {
        var q = _container.GetItemLinqQueryable<Book>(true)
                          .Where(b => b.userId == owner)
                          .ToFeedIterator();

        var results = new List<Book>();

        while (q.HasMoreResults)
        {
            results.AddRange(await q.ReadNextAsync());
        }

        return results;
    }

    public Task AddAsync(Book book) =>
        _container.CreateItemAsync(book, new PartitionKey(book.userId));

    public Task UpdateAsync(Book book) =>
        _container.UpsertItemAsync(book, new PartitionKey(book.userId));

    public Task DeleteAsync(string id, string owner) =>
        _container.DeleteItemAsync<Book>(id, new PartitionKey(owner));
}
