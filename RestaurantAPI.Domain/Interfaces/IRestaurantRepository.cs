using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Restaurant repository contract for restaurant-specific data operations.
/// </summary>
public interface IRestaurantRepository : IBaseRepository<Restaurant>
{
    /// <summary>
    /// Get restaurants filtered by category, address, and name.
    /// </summary>
    Task<IEnumerable<Restaurant>> GetByFiltersAsync(string category = "", string? address = null, string? name = null);

    /// <summary>
    /// Check if a restaurant exists by name.
    /// </summary>
    Task<bool> RestaurantExistsByNameAsync(string restaurantName);

    /// <summary>
    /// Get a restaurant with all its menu items included.
    /// </summary>
    Task<Restaurant?> GetWithItemsAsync(int restaurantId);
}
