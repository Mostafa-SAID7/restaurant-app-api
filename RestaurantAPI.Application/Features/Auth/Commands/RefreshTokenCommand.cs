using MediatR;
using RestaurantAPI.Auth.DTOs;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Command to refresh an expired JWT access token.
/// Implements refresh token rotation: old token invalidated, new pair issued.
/// Maps to: AuthService.RefreshAsync(refreshToken)
/// Returns: TokenResponseDto with new token pair
/// </summary>
public class RefreshTokenCommand : IRequest<TokenResponseDto>
{
    /// <summary>
    /// Valid refresh token to exchange for new token pair.
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}
