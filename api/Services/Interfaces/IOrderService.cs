using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Creates an order with proper transaction and identity handling.
    /// Returns OrderResponseDTO (no entity exposure).
    /// </summary>
    Task<OrderResponseDTO> CreateOrderAsync(int restaurantId, string apiKey, MenuDTO menuDTO);
    
    Task<IEnumerable<object>> GetUserOrdersAsync(string apiKey);
    Task<IEnumerable<Order>> GetOrdersByMasterIdAsync(string apiKey, int masterId);
    Task<bool> DeleteOrderAsync(int orderId, string apiKey);
    Task<object> DeleteMasterOrderAsync(int masterId, string apiKey);
}