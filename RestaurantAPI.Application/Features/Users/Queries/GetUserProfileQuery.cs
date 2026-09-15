using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Users.Queries;

/// <summary>
/// Query to get the current user's profile information.
/// Maps to: UserService.GetUserByIdAsync(userId)
/// </summary>
public class GetUserProfileQuery : IRequest<UserDTO?>
{
    /// <summary>
    /// User ID (from JWT claims, provided by controller).
    /// </summary>
    public string UserId { get; set; } = null!;
}
