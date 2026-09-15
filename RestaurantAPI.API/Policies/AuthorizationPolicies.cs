using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Auth.Policies;

/// <summary>
/// Authorization policy definitions for role-based access control.
/// These policies are configured in AuthServiceCollectionExtensions.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Policy name: User must be authenticated.
    /// Used for endpoints requiring login but no specific role.
    /// </summary>
    public const string Authenticated = "Authenticated";

    /// <summary>
    /// Policy name: User must have Admin role.
    /// Used for administrative operations.
    /// </summary>
    public const string AdminOnly = "AdminOnly";

    /// <summary>
    /// Policy name: User must have Customer or RestaurantOwner role.
    /// Used for user-specific operations (cart, orders, profile).
    /// </summary>
    public const string CustomerOrOwner = "CustomerOrOwner";

    /// <summary>
    /// Policy name: User must have RestaurantOwner role.
    /// Used for restaurant management endpoints.
    /// </summary>
    public const string OwnerOnly = "OwnerOnly";

    /// <summary>
    /// Policy name: User can modify their own resource.
    /// Requires resource-based authorization handler.
    /// </summary>
    public const string CanModifyOwnResource = "CanModifyOwnResource";

    /// <summary>
    /// Policy name: User can manage restaurant (owner or admin).
    /// Requires resource-based authorization handler.
    /// </summary>
    public const string CanManageRestaurant = "CanManageRestaurant";
}

/// <summary>
/// Requirement for resource-based authorization.
/// Verifies user owns or can modify the resource.
/// </summary>
public class CanModifyOwnResourceRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Handler for CanModifyOwnResourceRequirement.
/// Checks if the current user can modify the requested resource.
/// </summary>
public class CanModifyOwnResourceHandler : AuthorizationHandler<CanModifyOwnResourceRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanModifyOwnResourceRequirement requirement)
    {
        // Get the resource owner ID from route values
        var resourceOwnerId = context.Resource as string ?? 
                              (context.Resource as RouteValueDictionary)?["userId"] as string;

        // Get current user ID from claims
        var currentUserId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Allow if user owns resource or is admin
        if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(resourceOwnerId))
        {
            if (currentUserId == resourceOwnerId || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Requirement for managing restaurant operations.
/// </summary>
public class CanManageRestaurantRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Handler for CanManageRestaurantRequirement.
/// Checks if user is restaurant owner or admin.
/// </summary>
public class CanManageRestaurantHandler : AuthorizationHandler<CanManageRestaurantRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanManageRestaurantRequirement requirement)
    {
        // Allow if user is RestaurantOwner or Admin
        if (context.User.IsInRole("RestaurantOwner") || context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
