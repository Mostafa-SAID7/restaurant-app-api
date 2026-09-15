using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Order repository contract for order data operations.
/// </summary>
public interface IOrderRepository : IBaseRepository<Order>
{
    /// <summary>
    /// Get all orders for a user.
    /// </summary>
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Get all order lines for a specific master order.
    /// </summary>
    Task<IEnumerable<Order>> GetByMasterIdAsync(int masterId);
}
