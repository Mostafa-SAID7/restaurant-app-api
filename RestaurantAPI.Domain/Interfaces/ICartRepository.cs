using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Cart repository contract for shopping cart data operations.
/// </summary>
public interface ICartRepository : IBaseRepository<Cart>
{
    /// <summary>
    /// Get all cart items for a user.
    /// </summary>
    Task<IEnumerable<Cart>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Get a specific cart item by user and item ID.
    /// </summary>
    Task<Cart?> GetByUserAndItemAsync(string userId, int itemId);

    /// <summary>
    /// Remove a specific cart item by user and item ID.
    /// </summary>
    Task<bool> RemoveByUserAndItemAsync(string userId, int itemId);

    /// <summary>
    /// Clear all cart items for a user.
    /// </summary>
    Task<int> ClearByUserIdAsync(string userId);
}
