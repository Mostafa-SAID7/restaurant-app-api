using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Queries;

/// <summary>
/// Handler for GetCartItemsQuery.
/// Retrieves all items in the user's cart with calculated item totals.
/// </summary>
public class GetCartItemsQueryHandler : IRequestHandler<GetCartItemsQuery, IEnumerable<CartItemDTO>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCartItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CartItemDTO>> Handle(GetCartItemsQuery request, CancellationToken cancellationToken)
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

        // Map to DTOs with calculated totals
        return cartItems.Select(c => new CartItemDTO
        {
            CartID = c.CartID,
            ItemID = c.ItemID,
            ItemName = c.ItemName,
            ItemPrice = c.ItemPrice,
            Quantity = c.Quantity,
            TotalPrice = c.ItemPrice * c.Quantity
        }).ToList();
    }
}
