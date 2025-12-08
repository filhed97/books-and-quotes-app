using api.Models;

namespace api.Storage;

public interface IQuoteRepository
{
    Task<Quote?> GetAsync(string id);
    Task<List<Quote>> ListAsync();
    Task AddAsync(Quote quote);
    Task UpdateAsync(Quote quote);
    Task DeleteAsync(string id);
}
