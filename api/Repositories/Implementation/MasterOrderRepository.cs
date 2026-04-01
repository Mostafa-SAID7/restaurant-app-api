using RestuarantAPI.Data;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Repositories.Implementation;

/// <summary>
/// Master Order repository implementation with specific master order operations
/// </summary>
public class MasterOrderRepository : BaseRepository<MasterOrder>, IMasterOrderRepository
{
    public MasterOrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MasterOrder>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(m => m.UserID == userId)
            .ToListAsync();
    }

    public async Task<MasterOrder?> GetWithDetailsAsync(int masterId)
    {
        return await _dbSet
            .Include(m => m.user)
            .Include(m => m.restaurant)
            .FirstOrDefaultAsync(m => m.MasterID == masterId);
    }

    public async Task<IEnumerable<MasterOrder>> GetWithRestaurantByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(m => m.restaurant)
            .Include(m => m.user)
            .Where(m => m.UserID == userId)
            .ToListAsync();
    }
}