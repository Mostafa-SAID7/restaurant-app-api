using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Handler for AddToCartCommand.
/// Adds an item to the user's shopping cart with quantity and price snapshot.
/// </summary>
public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartItemDTO>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddToCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CartItemDTO> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("User ID is required");

        if (request.ItemId <= 0)
            throw new ArgumentException("Item ID must be greater than 0");

        if (request.Quantity < 1 || request.Quantity > 100)
            throw new ArgumentException("Quantity must be between 1 and 100");

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Verify item exists
        var item = await _unitOfWork.Items.GetByIdAsync(request.ItemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {request.ItemId} not found");

        // Create cart entry with price snapshot (immutable at time of add)
        var cartItem = new Cart
        {
            UserID = user.Usercode,
            ItemID = item.ItemID,
            ItemName = item.ItemName,
            ItemPrice = item.ItemPrice,
            Quantity = request.Quantity
        };

        // Persist
        await _unitOfWork.Carts.AddAsync(cartItem);
        await _unitOfWork.SaveChangesAsync();

        // Return DTO
        return new CartItemDTO
        {
            CartID = cartItem.CartID,
            ItemID = cartItem.ItemID,
            ItemName = cartItem.ItemName,
            ItemPrice = cartItem.ItemPrice,
            Quantity = cartItem.Quantity,
            TotalPrice = cartItem.ItemPrice * cartItem.Quantity
        };
    }
}
