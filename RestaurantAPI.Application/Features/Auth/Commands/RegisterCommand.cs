using MediatR;
using RestaurantAPI.Auth.DTOs;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Command to register a new user account.
/// Maps to: AuthService.RegisterAsync(email, password)
/// Returns: TokenResponseDto with initial access and refresh tokens
/// </summary>
public class RegisterCommand : IRequest<TokenResponseDto>
{
    /// <summary>
    /// User's email address (must be unique).
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password (plain text, will be hashed).
    /// Must meet password policy (minimum 12 characters).
    /// </summary>
    public string Password { get; set; } = null!;
}
