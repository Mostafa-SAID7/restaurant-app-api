namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Refresh token entity for token rotation and revocation.
/// Stores metadata for audit and rotation tracking.
/// Implements one-time use: once used to refresh, token is marked as used and new pair issued.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to User.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Hashed refresh token (bcrypt).
    /// Never store plaintext tokens; hash like passwords.
    /// </summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>
    /// Refresh token expiration time (typically 7 days from creation).
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When this token was created.
    /// Used for audit trail and rotation tracking.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Client IP address that created this token (for audit/security).
    /// Can detect token usage from unexpected locations.
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// When this token was revoked (logout, or replaced by newer token).
    /// Null = token not yet revoked.
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// When this token was used to refresh (token rotation).
    /// Null = token not yet used.
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Is this token currently active (not revoked, not expired).
    /// </summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Is this token expired (past expiration time).
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Is this token revoked.
    /// </summary>
    public bool IsRevoked => RevokedAt.HasValue;
}
