using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Refresh token repository contract for refresh token data operations.
/// </summary>
public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    /// <summary>
    /// Get a refresh token by its hashed value.
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Get all refresh tokens for a user.
    /// </summary>
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Revoke a refresh token by marking it as revoked.
    /// </summary>
    Task<bool> RevokeTokenAsync(string token);

    /// <summary>
    /// Check if a refresh token is valid (active and not expired).
    /// </summary>
    Task<bool> IsTokenValidAsync(string token);
}
