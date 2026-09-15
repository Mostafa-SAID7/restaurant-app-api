using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// Request DTO for user registration endpoint.
/// Contains credentials and user information for account creation.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// User email address.
    /// Must be unique across all users; used for login and recovery.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [MaxLength(500)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User password (plaintext; transmitted over HTTPS only).
    /// Will be hashed before storage using bcrypt.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(500, MinimumLength = 1)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Password confirmation (must match Password field).
    /// Client-side validation; server-side confirmation recommended.
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [StringLength(500, MinimumLength = 1)]
    public string ConfirmPassword { get; set; } = null!;
}

/// <summary>
/// Request DTO for user login endpoint.
/// Contains credentials required to authenticate user.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// User email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [MaxLength(500)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User password (plaintext; transmitted over HTTPS only).
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(500, MinimumLength = 1)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Optional: Device/client identifier for audit/security purposes.
    /// Can be used for multi-device login tracking.
    /// </summary>
    [StringLength(200)]
    public string? DeviceId { get; set; }
}

/// <summary>
/// Request DTO for token refresh endpoint.
/// Contains the refresh token presented by client to obtain new access token.
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// The refresh token from previous login/register response.
    /// Used to obtain new short-lived access token without re-authentication.
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    [StringLength(1000)]
    public string RefreshToken { get; set; } = null!;
}

/// <summary>
/// Request DTO for password change endpoint.
/// Requires authentication; user must provide current password + new password.
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Current password (for verification; must match stored hash).
    /// </summary>
    [Required(ErrorMessage = "Current password is required")]
    [StringLength(500, MinimumLength = 1)]
    public string CurrentPassword { get; set; } = null!;

    /// <summary>
    /// New password (must pass password policy).
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [StringLength(500, MinimumLength = 1)]
    public string NewPassword { get; set; } = null!;

    /// <summary>
    /// New password confirmation (must match NewPassword).
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [StringLength(500, MinimumLength = 1)]
    public string ConfirmNewPassword { get; set; } = null!;
}

// TokenResponseDto has been removed from this file.
// The single canonical definition lives in:
//   RestaurantAPI.Application.Common.Abstractions.ITokenService (alongside ITokenService)
// Import RestaurantAPI.Application.Common.Abstractions to use TokenResponseDto.
