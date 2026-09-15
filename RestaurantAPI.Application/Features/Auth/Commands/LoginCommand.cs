using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Command to authenticate a user with email and password.
/// Maps to: AuthService.LoginAsync(email, password)
/// Returns: TokenResponseDto with access and refresh tokens
/// </summary>
public class LoginCommand : IRequest<TokenResponseDto>
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password (plain text, hashed by service).
    /// </summary>
    public string Password { get; set; } = null!;
}
