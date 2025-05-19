namespace BookStoreApi.Features.Users;

public interface IUserService
{
    Task<User> RegisterAsync(User user, string password);
    Task<User?> GetByUsernameAsync(string username);
    Task<string> GenerateRefreshTokenAsync(User user);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task UpdateUserAsync(User user);
    Task<User?> LoginAsync(string username, string password);
    Task<User> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
} 