using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Auth.DTOs;

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
