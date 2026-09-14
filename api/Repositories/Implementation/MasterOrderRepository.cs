using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Repositories.Implementation;

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
            .Include(m => m.User)
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.MasterID == masterId);
    }

    public async Task<IEnumerable<MasterOrder>> GetWithRestaurantByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(m => m.Restaurant)
            .Include(m => m.User)
            .Where(m => m.UserID == userId)
            .ToListAsync();
    }
}