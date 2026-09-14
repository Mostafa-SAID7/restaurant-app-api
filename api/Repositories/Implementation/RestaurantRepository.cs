using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// Restaurant repository implementation.
/// Phase A.7: Pure data layer - filtering and querying only.
/// 
/// This repository handles:
/// - Querying restaurants with optional filters (category, address, name)
/// - Checking if restaurant exists by name
/// - Getting restaurant with related items
/// 
/// Filtering logic (Contains checks) is acceptable here because:
/// - It's a data-layer concern (WHAT data to retrieve)
/// - Not business logic (HOW to process/calculate data)
/// 
/// Business validation (e.g., "restaurant name must be unique") belongs in services.
/// </summary>
public class RestaurantRepository : BaseRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Restaurant>> GetByFiltersAsync(string category = "", string? address = null, string? name = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(r => r.Type.Contains(category));
        }

        if (!string.IsNullOrEmpty(address))
        {
            query = query.Where(r => r.Address.Contains(address));
        }

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(r => r.RestaurantName.Contains(name));
        }

        return await query.ToListAsync();
    }

    public async Task<bool> RestaurantExistsByNameAsync(string restaurantName)
    {
        return await _dbSet.AnyAsync(r => r.RestaurantName == restaurantName);
    }

    public async Task<Restaurant?> GetWithItemsAsync(int restaurantId)
    {
        var restaurant = await _dbSet.FirstOrDefaultAsync(r => r.RestaurantID == restaurantId);
        return restaurant;
    }
}