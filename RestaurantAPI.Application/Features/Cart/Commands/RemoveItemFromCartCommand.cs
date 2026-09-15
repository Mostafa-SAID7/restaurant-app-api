using MediatR;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Command to remove an item from the user's cart.
/// </summary>
public class RemoveItemFromCartCommand : IRequest<bool>
{
    public string UserId { get; set; } = null!;
    public int ItemId { get; set; }
}
