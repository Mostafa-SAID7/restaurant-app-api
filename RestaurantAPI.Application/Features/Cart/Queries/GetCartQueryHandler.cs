using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Queries;

/// <summary>
/// Handler for GetCartQuery.
/// Retrieves the user's complete cart with all items and calculated totals.
/// </summary>
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDTO>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCartQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDTO> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("User ID is required");

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Get all cart items for this user
        var cartItems = await _unitOfWork.Carts.GetByUserIdAsync(user.Usercode);

        // Calculate totals and map to DTOs
        var cartItemDtos = cartItems.Select(c => new CartItemDTO
        {
            CartID = c.CartID,
            ItemID = c.ItemID,
            ItemName = c.ItemName,
            ItemPrice = c.ItemPrice,
            Quantity = c.Quantity,
            TotalPrice = c.ItemPrice * c.Quantity
        }).ToList();

        var grandTotal = cartItemDtos.Sum(c => c.TotalPrice);

        return new CartDTO
        {
            CartItems = cartItemDtos,
            GrandTotal = grandTotal
        };
    }
}
