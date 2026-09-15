using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Command to add an item to the user's cart.
/// </summary>
public class AddItemToCartCommand : IRequest<CartItemDto>
{
    public string UserId { get; set; } = null!;
    public AddCartItemDto CartItem { get; set; } = new();
}
