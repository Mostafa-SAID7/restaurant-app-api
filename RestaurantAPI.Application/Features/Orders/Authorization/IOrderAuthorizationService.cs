namespace RestaurantAPI.Application.Features.Orders.Authorization;

/// <summary>
/// Authorization service for order operations.
/// Encapsulates all authorization logic for order access and modification.
/// This service is injected into command/query handlers to verify that operations
/// comply with business authorization rules (e.g., users can only modify their own orders).
/// 
/// PRINCIPLE: Separates authorization concerns from business logic.
/// This allows authorization policies to be tested, modified, and reused independently.
/// </summary>
public interface IOrderAuthorizationService
{
    /// <summary>
    /// Verifies that a user exists and is authorized to perform operations.
    /// Throws UnauthorizedAccessException if user is not found or cannot be authenticated.
    /// </summary>
    /// <param name="userId">The user identifier (typically from JWT claims).</param>
    /// <returns>The authenticated user entity if authorized.</returns>
    Task<Domain.Entities.User> AuthorizeUserAsync(string userId);

    /// <summary>
    /// Verifies that a user owns a specific order (by OrderID).
    /// Throws UnauthorizedAccessException if the order does not belong to the user.
    /// </summary>
    /// <param name="userId">The current user identifier.</param>
    /// <param name="orderId">The order line ID to check ownership of.</param>
    /// <returns>The order entity if authorized.</returns>
    Task<Domain.Entities.Order> AuthorizeOrderOwnershipAsync(string userId, int orderId);

    /// <summary>
    /// Verifies that a user owns a specific master order.
    /// Throws UnauthorizedAccessException if the master order does not belong to the user.
    /// </summary>
    /// <param name="userId">The current user identifier.</param>
    /// <param name="masterId">The master order ID to check ownership of.</param>
    /// <returns>The master order entity if authorized.</returns>
    Task<Domain.Entities.MasterOrder> AuthorizeMasterOrderOwnershipAsync(string userId, int masterId);
}
