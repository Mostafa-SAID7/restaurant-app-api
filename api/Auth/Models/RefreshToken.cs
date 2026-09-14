using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Auth.Models;

/// <summary>
/// Refresh token entity for token rotation and revocation.
/// Stores hashed refresh tokens with metadata for audit and rotation tracking.
/// Implements one-time use: once used to refresh, token is marked as used and new pair issued.
/// </summary>
[Table("RefreshTokens")]
public class RefreshToken
{
    /// <summary>
    /// Primary key.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to User entity.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Hashed refresh token (bcrypt).
    /// Never store plaintext tokens; hash like passwords.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string TokenHash { get; set; } = null!;

    /// <summary>
    /// Refresh token expiration time (typically 7 days from creation).
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When this token was created.
    /// Used for audit trail and rotation tracking.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Client IP address that created this token (for audit/security).
    /// Can detect token usage from unexpected locations.
    /// </summary>
    [MaxLength(45)] // IPv6 = max 45 chars
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
    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Is this token expired (past expiration time).
    /// </summary>
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Is this token revoked.
    /// </summary>
    [NotMapped]
    public bool IsRevoked => RevokedAt.HasValue;
}
