using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Master order repository contract for order summary/header data operations.
/// </summary>
public interface IMasterOrderRepository : IBaseRepository<MasterOrder>
{
    /// <summary>
    /// Get all master orders for a user.
    /// </summary>
    Task<IEnumerable<MasterOrder>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Get a master order with all related navigation properties.
    /// </summary>
    Task<MasterOrder?> GetWithDetailsAsync(int masterId);

    /// <summary>
    /// Get master orders with restaurant information for a user.
    /// </summary>
    Task<IEnumerable<MasterOrder>> GetWithRestaurantByUserIdAsync(string userId);
}
