using RestaurantAPI.Auth.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// User role repository interface for user-role associations
/// Phase A.6: Extracted from AppDbContext dependency in AuthService
/// </summary>
public interface IUserRoleRepository : IBaseRepository<ApplicationUserRole>
{
    /// <summary>
    /// Get all roles for a user
    /// </summary>
    Task<IEnumerable<ApplicationUserRole>> GetUserRolesByUserIdAsync(string userId);

    /// <summary>
    /// Check if user has a specific role
    /// </summary>
    Task<bool> UserHasRoleAsync(string userId, string roleName);

    /// <summary>
    /// Get user roles with role details (include Role)
    /// </summary>
    Task<IEnumerable<ApplicationUserRole>> GetUserRolesWithDetailsAsync(string userId);
}
