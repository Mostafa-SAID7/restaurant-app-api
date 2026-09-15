using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// User-Role repository contract for user role assignment operations.
/// </summary>
public interface IUserRoleRepository : IBaseRepository<ApplicationUserRole>
{
}
