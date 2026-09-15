using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _dbSet.Where(o => o.UserID == userId).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByMasterIdAsync(int masterId)
    {
        return await _dbSet.Where(o => o.MasterID == masterId).ToListAsync();
    }
}
