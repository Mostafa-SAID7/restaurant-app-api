using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for user registration/login requests
/// Phase B.1: Decouples external API contract from internal UserDTO
/// </summary>
public class ExternalUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    public string Password { get; set; }
}

/// <summary>
/// External DTO for password update requests
/// </summary>
public class ExternalPasswordUpdateRequest
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    public string ConfirmPassword { get; set; }
}
