using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Cart service for authenticated users
/// Phase A.1: Changed parameter from apiKey to userId (JWT)
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Get cart items for authenticated user (userId from JWT)
    /// </summary>
    Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(string userId);
    
    /// <summary>
    /// Add item to cart (userId from JWT)
    /// </summary>
    Task<CartItemDTO> AddItemToCartAsync(string userId, AddCartItemRequestDTO addCartItem);
    
    Task<bool> RemoveItemFromCartAsync(string userId, int itemId);
    
    /// <summary>
    /// Get cart summary with items and total (userId from JWT)
    /// </summary>
    Task<CartDTO> GetCartSummaryAsync(string userId);
    
    Task ClearCartAsync(string userId);
}