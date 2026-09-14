using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Auth.DTOs;

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
