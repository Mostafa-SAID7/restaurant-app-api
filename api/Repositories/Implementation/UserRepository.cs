using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// User repository for data access only
/// Phase A.2: Password verification removed (moved to IPasswordService)
/// Phase A.1: Usercode methods removed (JWT replaces API-key auth)
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

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.UserEmail == email);
    }
}