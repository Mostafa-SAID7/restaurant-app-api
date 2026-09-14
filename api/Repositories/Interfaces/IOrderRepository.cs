using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// Order repository interface with specific order operations
/// </summary>
public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Order>> GetByMasterIdAsync(int masterId);
}