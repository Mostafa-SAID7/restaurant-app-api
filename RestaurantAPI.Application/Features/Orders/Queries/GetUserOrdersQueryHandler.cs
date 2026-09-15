using MediatR;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Orders.Queries;

/// <summary>
/// Handler for GetUserOrdersQuery.
/// Retrieves user's orders without manual DTO construction.
/// </summary>
public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, GetUserOrdersResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserOrdersResponse> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (customer == null)
            throw new UnauthorizedAccessException("User not found");

        var masterOrders = await _unitOfWork.MasterOrders.GetWithRestaurantByUserIdAsync(customer.Usercode);

        // Return the first order as response (or you could make this return a list)
        var firstOrder = masterOrders.FirstOrDefault();
        if (firstOrder == null)
            throw new KeyNotFoundException("No orders found");

        return new GetUserOrdersResponse
        {
            MasterID = firstOrder.MasterID,
            UserCode = firstOrder.User?.Usercode,
            RestaurantID = firstOrder.RestaurantID,
            RestaurantName = firstOrder.Restaurant?.RestaurantName,
            GrandTotal = firstOrder.GrandTotal,
            CreatedAt = firstOrder.CreatedAt
        };
    }
}
