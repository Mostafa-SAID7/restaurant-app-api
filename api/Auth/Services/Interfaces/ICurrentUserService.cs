using System.Security.Claims;
using RestaurantAPI.Auth.Services.Interfaces;

namespace RestaurantAPI.Auth.Services.Interfaces;

/// <summary>
/// Service for accessing the current authenticated user's identity and claims.
/// Extracts user information from JWT claims in HttpContext.
/// Replaces HttpContext.Items["User"] manual lookups with abstraction.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current authenticated user's ID (subject claim).
    /// Returns null if user is not authenticated.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current authenticated user's email from claims.
    /// Returns null if email claim is not present.
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Gets the current authenticated user's roles (role claims).
    /// Returns empty collection if not authenticated or no role claims.
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Checks if current user has a specific role.
    /// </summary>
    /// <param name="role">Role name to check</param>
    /// <returns>True if user has role, false otherwise</returns>
    bool HasRole(string role);

    /// <summary>
    /// Checks if current user has any of the specified roles.
    /// </summary>
    /// <param name="roles">Roles to check</param>
    /// <returns>True if user has at least one role, false otherwise</returns>
    bool HasAnyRole(params string[] roles);

    /// <summary>
    /// Checks if current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets a specific claim value by type.
    /// </summary>
    /// <param name="claimType">Claim type to retrieve</param>
    /// <returns>Claim value, or null if not found</returns>
    string? GetClaimValue(string claimType);

    /// <summary>
    /// Gets all claims for current user.
    /// </summary>
    IEnumerable<Claim> GetClaims();
}
