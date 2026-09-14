using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Auth.Models;
using RestaurantAPI.Data;
using RestaurantAPI.Repositories.Interfaces;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// Role repository implementation for role data access
/// Phase A.6: Extracted from AppDbContext dependency in AuthService
/// </summary>
public class RoleRepository : BaseRepository<ApplicationRole>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get role by name
    /// </summary>
    public async Task<ApplicationRole?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    /// <summary>
    /// Check if role exists by name
    /// </summary>
    public async Task<bool> RoleExistsByNameAsync(string name)
    {
        return await _context.Roles.AnyAsync(r => r.Name == name);
    }
}
