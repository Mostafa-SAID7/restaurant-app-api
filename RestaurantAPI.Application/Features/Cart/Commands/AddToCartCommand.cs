using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Command to add an item to the user's cart.
/// Maps to: CartService.AddItemToCartAsync(userId, addCartItem)
/// </summary>
public class AddToCartCommand : IRequest<CartItemDTO>
{
    /// <summary>
    /// User ID (from JWT claims, provided by controller).
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Item ID to add to cart.
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Quantity to add (1-100).
    /// </summary>
    public int Quantity { get; set; }
}
