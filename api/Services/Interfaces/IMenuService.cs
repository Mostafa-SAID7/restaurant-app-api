using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Menu service for restaurant menu operations
/// Phase A.3: Extracted from RestaurantService for SRP
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// Get menu items for a specific restaurant
    /// </summary>
    Task<IEnumerable<GetItemsDTO>> GetMenuAsync(int restaurantId, string sortByPrice = "");
}
