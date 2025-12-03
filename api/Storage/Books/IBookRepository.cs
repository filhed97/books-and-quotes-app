using api.Models;

namespace api.Storage;

public interface IBookRepository
{
    Task<Book?> GetAsync(string id, string owner);
    Task<List<Book>> ListAsync(string owner);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(string id, string owner);
}