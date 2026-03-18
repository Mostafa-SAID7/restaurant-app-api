using FakeRestuarantAPI.Models;

namespace FakeRestuarantAPI.Repositories.Interfaces;

/// <summary>
/// Master Order repository interface with specific master order operations
/// </summary>
public interface IMasterOrderRepository : IBaseRepository<MasterOrder>
{
    Task<IEnumerable<MasterOrder>> GetByUserIdAsync(string userId);
    Task<MasterOrder?> GetWithDetailsAsync(int masterId);
    Task<IEnumerable<MasterOrder>> GetWithRestaurantByUserIdAsync(string userId);
}