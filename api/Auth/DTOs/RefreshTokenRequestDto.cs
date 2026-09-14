using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Auth.DTOs;

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
