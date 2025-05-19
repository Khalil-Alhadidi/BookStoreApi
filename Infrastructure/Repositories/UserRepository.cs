using Microsoft.EntityFrameworkCore;
using BookStoreApi.Features.Users;
using BookStoreApi.Infrastructure.Data;

namespace BookStoreApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookStoreDbContext _context;

    public UserRepository(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null)
            return false;

        // Update all properties
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
} 