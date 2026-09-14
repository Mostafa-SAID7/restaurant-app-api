using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Restaurant service for restaurant management operations
/// Phase A.3: Extracted menu/item operations to IMenuService and IItemService for SRP
/// </summary>
public interface IRestaurantService
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync(string category = "", string? address = null, string? name = null);
    Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId);
    Task<Restaurant> CreateRestaurantAsync(RestaurantDTO restaurantDTO);
    Task<bool> RestaurantExistsAsync(string restaurantName);
    Task UpdateImageUrlsAsync(int restaurantId, string[] urls);
}