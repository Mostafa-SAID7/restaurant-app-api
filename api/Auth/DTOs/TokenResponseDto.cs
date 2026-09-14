namespace RestaurantAPI.Auth.DTOs;

/// <summary>
/// Response DTO for successful authentication (login, register, refresh).
/// Contains JWT access token, refresh token, and user information.
/// Never contains password hash or other sensitive data.
/// </summary>
public class TokenResponseDto
{
    /// <summary>
    /// Short-lived JWT access token (15 minutes default).
    /// Use in Authorization header: Bearer {access_token}
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Long-lived refresh token (7 days default).
    /// Use to refresh access token without re-entering password.
    /// </summary>
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Token type (always "Bearer" for HTTP Bearer scheme).
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token expiration time in seconds.
    /// Typically 900 (15 minutes).
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// User identifier (Usercode).
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// User email address (non-sensitive).
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// User roles for client-side authorization checks.
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = [];
}
