using FakeRestuarantAPI.Models;

namespace FakeRestuarantAPI.Repositories.Interfaces;

/// <summary>
/// Cart repository interface with specific cart operations
/// </summary>
public interface ICartRepository : IBaseRepository<Cart>
{
    Task<IEnumerable<Cart>> GetByUserIdAsync(string userId);
    Task<Cart?> GetByUserAndItemAsync(string userId, int itemId);
    Task<int> ClearByUserIdAsync(string userId);
    Task<bool> RemoveByUserAndItemAsync(string userId, int itemId);
}