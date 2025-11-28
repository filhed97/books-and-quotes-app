public interface IUserRepository
{
    Task CreateUserAsync(string username, string passwordHash);
    Task<(string Username, string PasswordHash)?> GetUserAsync(string username);
}