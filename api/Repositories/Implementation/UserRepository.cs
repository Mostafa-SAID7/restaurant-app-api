using RestuarantAPI.Data;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Repositories.Implementation;

/// <summary>
/// User repository implementation with specific user operations
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

    public async Task<User?> GetByUserCodeAsync(string userCode)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Usercode == userCode);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.UserEmail == email);
    }

    public async Task<bool> UserCodeExistsAsync(string userCode)
    {
        return await _dbSet.AnyAsync(u => u.Usercode == userCode);
    }

    /// <summary>
    /// Validates user credentials by comparing hashed passwords
    /// </summary>
    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email);
        if (user == null)
            return null;

        // Verify password hash
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    /// <summary>
    /// Hashes a password for secure storage
    /// </summary>
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash);
        return result == PasswordVerificationResult.Success;
    }
}