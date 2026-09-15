using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(rt => rt.TokenHash == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId)
    {
        return await _dbSet.Where(rt => rt.UserId == userId).ToListAsync();
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken == null)
            return false;

        refreshToken.RevokedAt = DateTime.UtcNow;
        _dbSet.Update(refreshToken);
        return true;
    }

    public async Task<bool> IsTokenValidAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        return refreshToken != null && refreshToken.IsActive;
    }
}
