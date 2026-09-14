using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// User repository for data access only
/// Password hashing removed (Phase A.2 - moved to IPasswordService)
/// Usercode methods removed (Phase A.1 - JWT replaces API-key auth)
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly PasswordHasher<User> _passwordHasher;

    public UserRepository(AppDbContext context) : base(context)
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.UserEmail == email);
    }

    /// <summary>
    /// Validates user credentials for authentication (temporary - will move to IAuthService)
    /// </summary>
    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
        if (user == null)
            return null;

        // Verify password hash
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }
}