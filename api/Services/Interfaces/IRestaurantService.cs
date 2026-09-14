using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Restaurant service for restaurant management operations
/// Phase A.5: Returns DTOs only, never entities
/// </summary>
public interface IRestaurantService
{
    Task<IEnumerable<RestaurantDTO>> GetRestaurantsAsync(string category = "", string? address = null, string? name = null);
    Task<RestaurantDTO?> GetRestaurantByIdAsync(int restaurantId);
    Task<RestaurantDTO> CreateRestaurantAsync(RestaurantDTO restaurantDTO);
    Task<bool> RestaurantExistsAsync(string restaurantName);
    Task UpdateImageUrlsAsync(int restaurantId, string[] urls);
}