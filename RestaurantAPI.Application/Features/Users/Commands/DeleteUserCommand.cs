using MediatR;

namespace RestaurantAPI.Application.Features.Users.Commands;

/// <summary>
/// Command to delete a user account.
/// Maps to: UserService.DeleteUserAsync(userId)
/// </summary>
public class DeleteUserCommand : IRequest<bool>
{
    /// <summary>
    /// User ID to delete.
    /// </summary>
    public string UserId { get; set; } = null!;
}
