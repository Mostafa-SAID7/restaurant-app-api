using MediatR;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Command to clear all items from the user's cart.
/// Maps to: CartService.ClearCartAsync(userId)
/// </summary>
public class ClearCartCommand : IRequest<bool>
{
    /// <summary>
    /// User ID (from JWT claims, provided by controller).
    /// </summary>
    public string UserId { get; set; } = null!;
}
