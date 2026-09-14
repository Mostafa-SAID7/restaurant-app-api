using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Item service for menu item operations
/// Phase A.3: Extracted from RestaurantService for SRP
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Add a new item to a restaurant menu
    /// </summary>
    Task<Item> AddItemToMenuAsync(int restaurantId, ItemDTO itemDTO);

    /// <summary>
    /// Get all menu items (across all restaurants) with optional filtering
    /// </summary>
    Task<IEnumerable<GetItemsDTO>> GetAllItemsAsync(string itemName = "", string sortByPrice = "");
}
