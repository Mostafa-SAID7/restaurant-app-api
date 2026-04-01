using RestuarantAPI.Models;

namespace RestuarantAPI.Services.Interfaces;

public interface IRestaurantService
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync(string category = "", string? address = null, string? name = null);
    Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId);
    Task<Restaurant> CreateRestaurantAsync(RestaurantDTO restaurantDTO);
    Task<bool> RestaurantExistsAsync(string restaurantName);
    Task<IEnumerable<GetItems>> GetMenuAsync(int restaurantId, string sortByPrice = "");
    Task<Item> AddItemToMenuAsync(int restaurantId, ItemDTO itemDTO);
    Task<IEnumerable<GetItems>> GetAllItemsAsync(string itemName = "", string sortByPrice = "");
    Task UpdateImageUrlsAsync(int restaurantId, string[] urls);
}