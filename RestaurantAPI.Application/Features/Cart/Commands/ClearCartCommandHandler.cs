using MediatR;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Handler for ClearCartCommand.
/// Removes all items from the user's cart.
/// </summary>
public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClearCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("User ID is required");

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Clear cart
        await _unitOfWork.Carts.ClearByUserIdAsync(user.Usercode);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
