using RestuarantAPI.Data;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Repositories.Implementation;

/// <summary>
/// User repository implementation with specific user operations
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
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

    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserEmail == email && u.Password == password);
    }
}