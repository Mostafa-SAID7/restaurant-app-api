using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

/// <summary>
/// Cart repository implementation.
/// Phase A.7: Pure data layer - no business logic.
/// 
/// This repository handles:
/// - Querying cart items for a user
/// - Finding specific user-item combinations
/// - Clearing/removing cart items
/// 
/// Cart total calculations and quantity validation belong in services.
/// </summary>
public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cart>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(c => c.Item)
            .Include(c => c.User)
            .Where(c => c.UserID == userId)
            .ToListAsync();
    }

    public async Task<Cart?> GetByUserAndItemAsync(string userId, int itemId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.UserID == userId && c.ItemID == itemId);
    }

    public async Task<int> ClearByUserIdAsync(string userId)
    {
        var cartItems = await _dbSet
            .Where(c => c.UserID == userId)
            .ToListAsync();

        _dbSet.RemoveRange(cartItems);
        return cartItems.Count;
    }

    public async Task<bool> RemoveByUserAndItemAsync(string userId, int itemId)
    {
        var cartItem = await GetByUserAndItemAsync(userId, itemId);
        if (cartItem == null) return false;

        _dbSet.Remove(cartItem);
        return true;
    }
}