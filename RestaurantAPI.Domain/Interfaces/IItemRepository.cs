using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Item repository contract for item/menu-specific data operations.
/// </summary>
public interface IItemRepository : IBaseRepository<Item>
{
    /// <summary>
    /// Get all items for a restaurant.
    /// </summary>
    Task<IEnumerable<Item>> GetByRestaurantIdAsync(int restaurantId);

    /// <summary>
    /// Get items filtered by name and optionally sorted by price.
    /// </summary>
    Task<IEnumerable<Item>> GetByFiltersAsync(string itemName = "", string sortByPrice = "");

    /// <summary>
    /// Get restaurant menu sorted optionally by price.
    /// </summary>
    Task<IEnumerable<Item>> GetMenuByRestaurantIdAsync(int restaurantId, string sortByPrice = "");

    /// <summary>
    /// Get a specific item by name and restaurant.
    /// </summary>
    Task<Item?> GetByNameAndRestaurantAsync(string itemName, int restaurantId);
}
