using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Item service for menu item operations
/// Phase A.5: Returns DTOs only, never entities
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Add a new item to a restaurant menu
    /// </summary>
    Task<ItemDTO> AddItemToMenuAsync(int restaurantId, ItemDTO itemDTO);

    /// <summary>
    /// Get all menu items (across all restaurants) with optional filtering
    /// </summary>
    Task<IEnumerable<ItemResponseDTO>> GetAllItemsAsync(string itemName = "", string sortByPrice = "");
}
