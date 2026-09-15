using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Cart>> GetByUserIdAsync(string userId)
    {
        return await _dbSet.Where(c => c.UserID == userId).ToListAsync();
    }

    public async Task<Cart?> GetByUserAndItemAsync(string userId, int itemId)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.UserID == userId && c.ItemID == itemId);
    }

    public async Task<bool> RemoveByUserAndItemAsync(string userId, int itemId)
    {
        var cart = await GetByUserAndItemAsync(userId, itemId);
        if (cart == null)
            return false;

        _dbSet.Remove(cart);
        return true;
    }

    public async Task<int> ClearByUserIdAsync(string userId)
    {
        var carts = await _dbSet.Where(c => c.UserID == userId).ToListAsync();
        _dbSet.RemoveRange(carts);
        return carts.Count;
    }
}
