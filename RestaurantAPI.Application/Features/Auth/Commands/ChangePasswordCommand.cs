using MediatR;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Command to change the current user's password.
/// Requires authentication; user provides current and new password.
/// New password must pass policy validation (minimum 12 characters).
/// Maps to: AuthService.ChangePasswordAsync(userId, currentPassword, newPassword)
/// Returns: bool (success indicator)
/// </summary>
public class ChangePasswordCommand : IRequest<bool>
{
    /// <summary>
    /// User ID (from JWT claims).
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Current password for verification.
    /// </summary>
    public string CurrentPassword { get; set; } = null!;

    /// <summary>
    /// New password to set (must meet policy).
    /// </summary>
    public string NewPassword { get; set; } = null!;
}
