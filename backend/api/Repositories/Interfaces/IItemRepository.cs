using FakeRestuarantAPI.Models;

namespace FakeRestuarantAPI.Repositories.Interfaces;

/// <summary>
/// Item repository interface with specific item operations
/// </summary>
public interface IItemRepository : IBaseRepository<Item>
{
    Task<IEnumerable<Item>> GetByRestaurantIdAsync(int restaurantId);
    Task<IEnumerable<Item>> GetByFiltersAsync(string itemName = "", string sortByPrice = "");
    Task<IEnumerable<Item>> GetMenuByRestaurantIdAsync(int restaurantId, string sortByPrice = "");
    Task<Item?> GetByNameAndRestaurantAsync(string itemName, int restaurantId);
}