using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Cart.Queries;

/// <summary>
/// Query to get the user's cart summary with all items and total.
/// Maps to: CartService.GetCartSummaryAsync(userId)
/// </summary>
public class GetCartQuery : IRequest<CartDto>
{
    /// <summary>
    /// User ID (from JWT claims, provided by controller).
    /// </summary>
    public string UserId { get; set; } = null!;
}
