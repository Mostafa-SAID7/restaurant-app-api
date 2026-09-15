using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Users.Queries;

/// <summary>
/// Query to get all registered users.
/// SECURITY WARNING: This should only be accessible to admins.
/// Maps to: UserService.GetAllUsersAsync()
/// </summary>
public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
    // No parameters - returns all users (access should be restricted via authorization policy)
}
