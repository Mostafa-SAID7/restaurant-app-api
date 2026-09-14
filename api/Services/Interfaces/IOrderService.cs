using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// Order service for creating and managing orders
/// Phase A.5: Returns DTOs only, never entities
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates an order with proper transaction handling
    /// userId from JWT claims instead of apiKey (Phase A.1)
    /// Phase A.8: Uses CreateOrderRequestDTO for clear naming
    /// </summary>
    Task<OrderResponseDTO> CreateOrderAsync(int restaurantId, string userId, CreateOrderRequestDTO createOrderRequest);
    
    Task<IEnumerable<object>> GetUserOrdersAsync(string userId);
    Task<IEnumerable<OrderDTO>> GetOrdersByMasterIdAsync(string userId, int masterId);
    Task<bool> DeleteOrderAsync(int orderId, string userId);
    Task<object> DeleteMasterOrderAsync(int masterId, string userId);
}