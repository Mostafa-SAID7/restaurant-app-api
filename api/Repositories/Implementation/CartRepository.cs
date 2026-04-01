using RestuarantAPI.Data;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Repositories.Implementation;

/// <summary>
/// Cart repository implementation with specific cart operations
/// </summary>
public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cart>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(c => c.item)
            .Include(c => c.user)
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