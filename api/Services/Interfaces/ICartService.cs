using RestuarantAPI.Models;

namespace RestuarantAPI.Services.Interfaces;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetCartItemsAsync(string apiKey);
    Task<CartDTO> AddItemToCartAsync(string apiKey, setcart setCart);
    Task<bool> RemoveItemFromCartAsync(string apiKey, int itemId);
    Task<getcartDTO> GetCartSummaryAsync(string apiKey);
    Task ClearCartAsync(string apiKey);
}