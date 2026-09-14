using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// Item repository implementation.
/// Phase A.7: Pure data layer - filtering, sorting, and querying only.
/// 
/// This repository handles:
/// - Querying items by restaurant
/// - Querying items with optional name filter and price sorting
/// - Finding specific items by name and restaurant
/// 
/// Sorting and filtering are data-layer concerns (what to retrieve).
/// Item pricing calculations and discounts belong in services.
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
            .Include(i => i.Restaurant)
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