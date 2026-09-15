using MediatR;
using RestaurantAPI.Application.Features.Orders.Authorization;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Handler for DeleteMasterOrderCommand.
/// Deletes a master order and all its associated line items with transaction.
/// 
/// Authorization: Uses IOrderAuthorizationService to verify user owns the order before deletion.
/// This separates authorization concerns from business logic and enables reusable authorization policies.
/// </summary>
public class DeleteMasterOrderCommandHandler : IRequestHandler<DeleteMasterOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderAuthorizationService _orderAuthorizationService;

    public DeleteMasterOrderCommandHandler(IUnitOfWork unitOfWork, IOrderAuthorizationService orderAuthorizationService)
    {
        _unitOfWork = unitOfWork;
        _orderAuthorizationService = orderAuthorizationService;
    }

    public async Task<bool> Handle(DeleteMasterOrderCommand request, CancellationToken cancellationToken)
    {
        // Verify user is authorized and owns the master order
        await _orderAuthorizationService.AuthorizeMasterOrderOwnershipAsync(request.UserId, request.MasterId);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var masterOrder = await _unitOfWork.MasterOrders.GetWithDetailsAsync(request.MasterId);
            if (masterOrder == null)
                throw new ArgumentException("Master order not found");

            var orderLines = await _unitOfWork.Orders.GetByMasterIdAsync(request.MasterId);

            await _unitOfWork.MasterOrders.DeleteAsync(masterOrder);
            await _unitOfWork.Orders.DeleteRangeAsync(orderLines);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
