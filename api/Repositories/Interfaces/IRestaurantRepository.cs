using FakeRestuarantAPI.Models;

namespace FakeRestuarantAPI.Repositories.Interfaces;

/// <summary>
/// Restaurant repository interface with specific restaurant operations
/// </summary>
public interface IRestaurantRepository : IBaseRepository<Restaurant>
{
    Task<IEnumerable<Restaurant>> GetByFiltersAsync(string category = "", string? address = null, string? name = null);
    Task<bool> RestaurantExistsByNameAsync(string restaurantName);
    Task<Restaurant?> GetWithItemsAsync(int restaurantId);
}