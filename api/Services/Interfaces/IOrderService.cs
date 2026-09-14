using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Order service for creating and managing orders
/// Phase A.1: Changed parameters from apiKey to userId (JWT)
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates an order with proper transaction handling
    /// userId from JWT claims instead of apiKey (Phase A.1)
    /// </summary>
    Task<OrderResponseDTO> CreateOrderAsync(int restaurantId, string userId, MenuDTO menuDTO);
    
    Task<IEnumerable<object>> GetUserOrdersAsync(string userId);
    Task<IEnumerable<Order>> GetOrdersByMasterIdAsync(string userId, int masterId);
    Task<bool> DeleteOrderAsync(int orderId, string userId);
    Task<object> DeleteMasterOrderAsync(int masterId, string userId);
}