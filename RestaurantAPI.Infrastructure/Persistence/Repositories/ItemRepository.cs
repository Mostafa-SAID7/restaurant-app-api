using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Item>> GetByRestaurantIdAsync(int restaurantId)
    {
        return await _dbSet.Where(i => i.RestaurantID == restaurantId).ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetByFiltersAsync(string itemName = "", string sortByPrice = "")
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(itemName))
            query = query.Where(i => i.ItemName.Contains(itemName));

        if (sortByPrice.ToLower() == "asc")
            query = query.OrderBy(i => i.ItemPrice);
        else if (sortByPrice.ToLower() == "desc")
            query = query.OrderByDescending(i => i.ItemPrice);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetMenuByRestaurantIdAsync(int restaurantId, string sortByPrice = "")
    {
        var query = _dbSet.Where(i => i.RestaurantID == restaurantId);

        if (sortByPrice.ToLower() == "asc")
            query = query.OrderBy(i => i.ItemPrice);
        else if (sortByPrice.ToLower() == "desc")
            query = query.OrderByDescending(i => i.ItemPrice);

        return await query.ToListAsync();
    }

    public async Task<Item?> GetByNameAndRestaurantAsync(string itemName, int restaurantId)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.ItemName == itemName && i.RestaurantID == restaurantId);
    }
}
