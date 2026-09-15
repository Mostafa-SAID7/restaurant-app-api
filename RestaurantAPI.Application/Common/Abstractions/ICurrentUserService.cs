namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Port for accessing the currently authenticated user information.
/// Abstraction over HTTP context to allow testing and separation of concerns.
/// Implementation lives in WebApi/Infrastructure layer.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get the current user's ID from the authentication context.
    /// </summary>
    string GetUserId();

    /// <summary>
    /// Get the current user's email from the authentication context.
    /// </summary>
    string GetUserEmail();

    /// <summary>
    /// Check if a user is authenticated.
    /// </summary>
    bool IsAuthenticated();
}
