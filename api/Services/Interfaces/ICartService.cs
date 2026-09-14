using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

public interface ICartService
{
    /// <summary>
    /// Get cart items for the authenticated user (returns CartItemDTO, not Cart entities)
    /// </summary>
    Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(string apiKey);
    
    /// <summary>
    /// Add item to cart (returns CartItemDTO)
    /// </summary>
    Task<CartItemDTO> AddItemToCartAsync(string apiKey, SetCart setCart);
    
    Task<bool> RemoveItemFromCartAsync(string apiKey, int itemId);
    
    /// <summary>
    /// Get cart summary with items (returns CartItemDTO, not Cart entities)
    /// </summary>
    Task<GetCartDTO> GetCartSummaryAsync(string apiKey);
    
    Task ClearCartAsync(string apiKey);
}