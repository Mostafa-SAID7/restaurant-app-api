using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Orders.Authorization;

/// <summary>
/// Implementation of IOrderAuthorizationService.
/// Verifies authorization for order operations: user existence, order ownership.
/// 
/// This service centralizes all authorization logic for the Orders feature.
/// It is injected into command/query handlers to enforce authorization before
/// processing business logic. This design separates authorization concerns from
/// business logic, making both easier to test and modify.
/// </summary>
public class OrderAuthorizationService : IOrderAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderAuthorizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> AuthorizeUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User ID cannot be empty");

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        return user;
    }

    public async Task<Order> AuthorizeOrderOwnershipAsync(string userId, int orderId)
    {
        // First verify the user exists
        var user = await AuthorizeUserAsync(userId);

        // Then verify the order belongs to this user
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException($"Order with ID {orderId} not found");

        if (order.UserID != user.Usercode)
            throw new UnauthorizedAccessException("You do not have permission to access this order");

        return order;
    }

    public async Task<MasterOrder> AuthorizeMasterOrderOwnershipAsync(string userId, int masterId)
    {
        // First verify the user exists
        var user = await AuthorizeUserAsync(userId);

        // Then verify the master order belongs to this user
        var masterOrder = await _unitOfWork.MasterOrders.GetByIdAsync(masterId);
        if (masterOrder == null)
            throw new ArgumentException($"Master order with ID {masterId} not found");

        if (masterOrder.UserID != user.Usercode)
            throw new UnauthorizedAccessException("You do not have permission to access this order");

        return masterOrder;
    }
}
