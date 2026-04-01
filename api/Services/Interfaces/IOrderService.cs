using RestuarantAPI.Models;

namespace RestuarantAPI.Services.Interfaces;

public interface IOrderService
{
    Task<FullOrderDTO> CreateOrderAsync(int restaurantId, string apiKey, MenuDTO menuDTO);
    Task<IEnumerable<object>> GetUserOrdersAsync(string apiKey);
    Task<IEnumerable<Order>> GetOrdersByMasterIdAsync(string apiKey, int masterId);
    Task<bool> DeleteOrderAsync(int orderId, string apiKey);
    Task<object> DeleteMasterOrderAsync(int masterId, string apiKey);
}