using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// User-Role repository contract for user role assignment operations.
/// </summary>
public interface IUserRoleRepository : IBaseRepository<ApplicationUserRole>
{
    /// <summary>
    /// Get user roles with role details (name, permissions, etc.) for a given user.
    /// </summary>
    Task<List<ApplicationUserRole>> GetUserRolesWithDetailsAsync(string userCode);
}
