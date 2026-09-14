using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// User repository for data access only.
/// Phase A.7: Pure data layer - no business logic.
/// 
/// This repository handles:
/// - Querying users by email (for auth lookups)
/// - Checking email existence (for duplicate prevention)
/// 
/// This repository does NOT:
/// - Validate user data (belongs in services)
/// - Hash passwords (belongs in IPasswordService)
/// - Check permissions (belongs in services)
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