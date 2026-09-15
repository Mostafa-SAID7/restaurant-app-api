using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Role repository contract for role management data operations.
/// </summary>
public interface IRoleRepository : IBaseRepository<ApplicationRole>
{
    /// <summary>
    /// Get a role by its name.
    /// </summary>
    Task<ApplicationRole?> GetByNameAsync(string name);

    /// <summary>
    /// Check if a role exists by name.
    /// </summary>
    Task<bool> RoleExistsByNameAsync(string name);
}
