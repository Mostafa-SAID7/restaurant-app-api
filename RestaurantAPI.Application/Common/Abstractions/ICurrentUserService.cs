namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Port for accessing the currently authenticated user information from HTTP context.
/// Abstraction over HttpContext to allow testing and separation of concerns.
/// Implementation lives in Infrastructure layer (accesses ClaimsPrincipal).
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get the current user's ID (Usercode) from JWT claims.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Get the current user's email from JWT claims.
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Get the current user's roles from JWT claims.
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Check if user has a specific role.
    /// </summary>
    bool HasRole(string role);

    /// <summary>
    /// Check if user has any of the specified roles.
    /// </summary>
    bool HasAnyRole(params string[] roles);

    /// <summary>
    /// Check if user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Get a specific claim value by claim type.
    /// </summary>
    string? GetClaimValue(string claimType);

    /// <summary>
    /// Get all claims for the current user.
    /// </summary>
    IEnumerable<System.Security.Claims.Claim> GetClaims();
}
