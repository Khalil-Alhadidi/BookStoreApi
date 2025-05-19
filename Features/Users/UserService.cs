using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Features.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<User?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _userManager.FindByNameAsync(usernameOrEmail) ?? await _userManager.FindByEmailAsync(usernameOrEmail);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UsernameOrEmail}", usernameOrEmail);
            return null;
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Invalid password for user: {UsernameOrEmail}", usernameOrEmail);
            return null;
        }
        return user;
    }

    public async Task<User> RegisterAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username) => await _userManager.FindByNameAsync(username);

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);
        return refreshToken;
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return (await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow));
    }

    public async Task UpdateUserAsync(User user) => await _userManager.UpdateAsync(user);

    public async Task<User> GetByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return _userManager.Users.ToList();
    }

    public async Task UpdateAsync(User user)
    {
        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }
} 