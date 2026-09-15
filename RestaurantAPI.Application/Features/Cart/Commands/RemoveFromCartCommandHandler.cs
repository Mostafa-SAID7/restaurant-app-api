using MediatR;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Handler for RemoveFromCartCommand.
/// Removes a specific item from the user's cart.
/// </summary>
public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("User ID is required");

        if (request.ItemId <= 0)
            throw new ArgumentException("Item ID must be greater than 0");

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Remove item from cart
        var removed = await _unitOfWork.Carts.RemoveByUserAndItemAsync(user.Usercode, request.ItemId);
        if (removed)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return removed;
    }
}
