using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class RestaurantRepository : BaseRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Restaurant>> GetByFiltersAsync(string category = "", string? address = null, string? name = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(r => r.Type.Contains(category));

        if (!string.IsNullOrEmpty(address))
            query = query.Where(r => r.Address.Contains(address));

        if (!string.IsNullOrEmpty(name))
            query = query.Where(r => r.RestaurantName.Contains(name));

        return await query.ToListAsync();
    }

    public async Task<bool> RestaurantExistsByNameAsync(string restaurantName)
    {
        return await _dbSet.AnyAsync(r => r.RestaurantName == restaurantName);
    }

    public async Task<Restaurant?> GetWithItemsAsync(int restaurantId)
    {
        return await _dbSet
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.RestaurantID == restaurantId);
    }
}
