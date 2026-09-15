using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Cart.Queries;

/// <summary>
/// Query to get all items in the user's cart (without summary totals).
/// Maps to: CartService.GetCartItemsAsync(userId)
/// </summary>
public class GetCartItemsQuery : IRequest<IEnumerable<CartItemDTO>>
{
    /// <summary>
    /// User ID (from JWT claims, provided by controller).
    /// </summary>
    public string UserId { get; set; } = null!;
}
