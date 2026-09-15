using MediatR;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Handler for DeleteOrderCommand.
/// Deletes a single order line.
/// </summary>
public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (customer == null)
            return false;

        var deleted = await _unitOfWork.Orders.DeleteAsync(request.OrderId);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return deleted;
    }
}
