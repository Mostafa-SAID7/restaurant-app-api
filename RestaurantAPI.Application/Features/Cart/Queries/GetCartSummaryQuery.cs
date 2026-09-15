using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Cart.Queries;

/// <summary>
/// Query to get the complete cart summary for a user.
/// </summary>
public class GetCartSummaryQuery : IRequest<CartDto>
{
    public string UserId { get; set; } = null!;
}
