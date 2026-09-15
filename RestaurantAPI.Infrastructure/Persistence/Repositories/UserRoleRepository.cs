using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class UserRoleRepository : BaseRepository<ApplicationUserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }

    /// <summary>
    /// Get user roles with role details for a given user.
    /// </summary>
    public async Task<List<ApplicationUserRole>> GetUserRolesWithDetailsAsync(string userCode)
    {
        return await _context.Set<ApplicationUserRole>()
            .Where(ur => ur.User.Usercode == userCode)
            .Include(ur => ur.Role)
            .ToListAsync();
    }
}
