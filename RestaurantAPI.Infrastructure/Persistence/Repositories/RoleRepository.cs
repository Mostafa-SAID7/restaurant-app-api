using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class RoleRepository : BaseRepository<ApplicationRole>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<ApplicationRole?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<bool> RoleExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(r => r.Name == name);
    }
}
