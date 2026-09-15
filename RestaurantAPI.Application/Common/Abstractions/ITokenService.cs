using System.Security.Claims;

namespace RestaurantAPI.Auth.Services.Interfaces;

/// <summary>
/// Service for JWT token generation, validation, and refresh token management.
/// Responsible for creating short-lived access tokens and long-lived refresh tokens.
/// Implements separation of concerns: token lifecycle management only.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a new JWT access token and refresh token pair for authenticated user.
    /// </summary>
    /// <param name="userId">Unique user identifier (Usercode or claim)</param>
    /// <param name="userEmail">User email address for token claims</param>
    /// <param name="roles">List of user roles for RBAC claims</param>
    /// <returns>Token response with access token, refresh token, and expiration info</returns>
    Task<TokenResponse> GenerateTokensAsync(string userId, string userEmail, IEnumerable<string> roles);

    /// <summary>
    /// Generates a new access token from a valid refresh token.
    /// Implements refresh token rotation: old token invalidated, new pair issued.
    /// </summary>
    /// <param name="refreshToken">The refresh token presented by client</param>
    /// <returns>New token pair, or null if refresh token is invalid/expired/revoked</returns>
    Task<TokenResponse?> RefreshAccessTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes a refresh token (marks as used, invalidates for future use).
    /// Called on logout to prevent token replay.
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke</param>
    Task RevokeRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Validates a JWT token without relying on .NET's built-in validation.
    /// Used for custom token verification logic.
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Principal if valid, null otherwise</returns>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}

/// <summary>
/// Response DTO containing access token, refresh token, and metadata.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// Short-lived JWT access token (15-30 minutes).
    /// Use in Authorization: Bearer header for API requests.
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Long-lived refresh token (7 days).
    /// Use to obtain new access token pair without re-entering credentials.
    /// </summary>
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Token type (always "Bearer" for OAuth2 compatibility).
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token expiration in seconds (typically 900 = 15 minutes).
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Unix timestamp when access token expires (for client-side tracking).
    /// </summary>
    public long ExpiresAt { get; set; }
}
