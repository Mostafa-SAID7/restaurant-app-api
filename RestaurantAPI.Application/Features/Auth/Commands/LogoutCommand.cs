using MediatR;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Command to logout a user by revoking their refresh token.
/// Prevents future token refreshes with the revoked token.
/// Maps to: AuthService.LogoutAsync(refreshToken)
/// Returns: bool (success indicator)
/// </summary>
public class LogoutCommand : IRequest<bool>
{
    /// <summary>
    /// Refresh token to revoke/invalidate.
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}
