using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Auth.Models;
using RestaurantAPI.Data;
using RestaurantAPI.Repositories.Interfaces;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// User role repository implementation for user-role association data access
/// Phase A.6: Extracted from AppDbContext dependency in AuthService
/// </summary>
public class UserRoleRepository : BaseRepository<ApplicationUserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get all roles for a user
    /// </summary>
    public async Task<IEnumerable<ApplicationUserRole>> GetUserRolesByUserIdAsync(string userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Check if user has a specific role
    /// </summary>
    public async Task<bool> UserHasRoleAsync(string userId, string roleName)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.Role!.Name == roleName);
    }

    /// <summary>
    /// Get user roles with role details (include Role navigation property)
    /// </summary>
    public async Task<IEnumerable<ApplicationUserRole>> GetUserRolesWithDetailsAsync(string userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ToListAsync();
    }
}
