using RestaurantAPI.Auth.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// Refresh token repository interface for token management
/// Phase A.6: Extracted from AppDbContext dependency in AuthService
/// </summary>
public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    /// <summary>
    /// Get refresh token by token value
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Get all refresh tokens for a user
    /// </summary>
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Revoke (delete) a refresh token
    /// </summary>
    Task<bool> RevokeTokenAsync(string token);

    /// <summary>
    /// Check if token exists and is not expired
    /// </summary>
    Task<bool> IsTokenValidAsync(string token);
}
