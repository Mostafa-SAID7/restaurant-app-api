using RestaurantAPI.Auth.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// Role repository interface for role management
/// Phase A.6: Extracted from AppDbContext dependency in AuthService
/// </summary>
public interface IRoleRepository : IBaseRepository<ApplicationRole>
{
    /// <summary>
    /// Get role by name
    /// </summary>
    Task<ApplicationRole?> GetByNameAsync(string name);

    /// <summary>
    /// Check if role exists by name
    /// </summary>
    Task<bool> RoleExistsByNameAsync(string name);
}
