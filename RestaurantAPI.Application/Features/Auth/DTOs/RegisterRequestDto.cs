using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Auth.DTOs;

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
