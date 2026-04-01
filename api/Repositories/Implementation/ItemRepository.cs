using RestuarantAPI.Data;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Repositories.Implementation;

/// <summary>
/// Item repository implementation with specific item operations
/// </summary>
public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Item>> GetByRestaurantIdAsync(int restaurantId)
    {
        return await _dbSet
            .Where(i => i.RestaurantID == restaurantId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetByFiltersAsync(string itemName = "", string sortByPrice = "")
    {
        var query = _dbSet
            .Include(i => i.restaurant)
            .AsQueryable();

        if (!string.IsNullOrEmpty(itemName))
        {
            query = query.Where(i => i.ItemName.Contains(itemName));
        }

        // Apply sorting
        query = sortByPrice.ToLower() switch
        {
            "asc" => query.OrderBy(i => i.ItemPrice),
            "desc" => query.OrderByDescending(i => i.ItemPrice),
            _ => query
        };

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetMenuByRestaurantIdAsync(int restaurantId, string sortByPrice = "")
    {
        var query = _dbSet
            .Where(i => i.RestaurantID == restaurantId)
            .AsQueryable();

        // Apply sorting
        query = sortByPrice.ToLower() switch
        {
            "asc" => query.OrderBy(i => i.ItemPrice),
            "desc" => query.OrderByDescending(i => i.ItemPrice),
            _ => query
        };

        return await query.ToListAsync();
    }

    public async Task<Item?> GetByNameAndRestaurantAsync(string itemName, int restaurantId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.ItemName == itemName && i.RestaurantID == restaurantId);
    }
}