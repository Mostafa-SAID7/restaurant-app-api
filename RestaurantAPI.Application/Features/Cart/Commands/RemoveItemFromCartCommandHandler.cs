using MediatR;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Handler for RemoveItemFromCartCommand.
/// Removes a specific item from user's cart.
/// </summary>
public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItemFromCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            return false;

        var removed = await _unitOfWork.Carts.RemoveByUserAndItemAsync(user.Usercode, request.ItemId);
        if (removed)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return removed;
    }
}
