using FakeRestuarantAPI.Data;
using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FakeRestuarantAPI.Repositories.Implementation;

/// <summary>
/// Order repository implementation with specific order operations
/// </summary>
public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(o => o.UserID == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByMasterIdAsync(int masterId)
    {
        return await _dbSet
            .Where(o => o.MasterID == masterId)
            .ToListAsync();
    }

    public async Task<int> GetNextMasterIdAsync()
    {
        var lastMasterId = await _context.masterOrders
            .OrderByDescending(m => m.MasterID)
            .Select(m => m.MasterID)
            .FirstOrDefaultAsync();

        return lastMasterId + 1;
    }
}