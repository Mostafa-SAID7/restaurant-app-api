using MediatR;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Command to remove an item from the user's cart.
/// Maps to: CartService.RemoveItemFromCartAsync(userId, itemId)
/// </summary>
public class RemoveFromCartCommand : IRequest<bool>
{
    /// <summary>
    /// User ID (from JWT claims, provided by controller).
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Item ID to remove from cart.
    /// </summary>
    public int ItemId { get; set; }
}
