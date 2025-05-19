namespace BookStoreApi.Features.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
} 