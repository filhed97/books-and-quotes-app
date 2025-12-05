using api.Models;

namespace api.Storage;

public interface IQuoteRepository
{
    Task<Quote?> GetAsync(string id, string owner);
    Task<List<Quote>> ListAsync(string owner);
    Task AddAsync(Quote quote);
    Task UpdateAsync(Quote quote);
    Task DeleteAsync(string id, string owner);
}