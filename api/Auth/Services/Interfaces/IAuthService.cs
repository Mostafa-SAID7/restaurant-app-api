namespace RestaurantAPI.Auth.Services.Interfaces;

/// <summary>
/// Orchestration service for authentication operations: register, login, refresh, logout.
/// Coordinates PasswordService, TokenService, and repository access.
/// Single Responsibility: orchestrate auth workflows only.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with email and password.
    /// Validates password policy, checks email uniqueness, hashes password, stores user.
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="password">Plaintext password (will be hashed)</param>
    /// <returns>AuthResult with user details and tokens on success</returns>
    Task<AuthResult> RegisterAsync(string email, string password);

    /// <summary>
    /// Authenticates user and issues access + refresh token pair.
    /// Validates credentials, implements account lockout after failed attempts.
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="password">Plaintext password</param>
    /// <returns>AuthResult with tokens on success, errors on failure</returns>
    Task<AuthResult> LoginAsync(string email, string password);

    /// <summary>
    /// Refreshes access token using a valid refresh token.
    /// Implements refresh token rotation: old token invalidated, new pair issued.
    /// </summary>
    /// <param name="refreshToken">Refresh token presented by client</param>
    /// <returns>AuthResult with new token pair on success</returns>
    Task<AuthResult> RefreshAsync(string refreshToken);

    /// <summary>
    /// Logs out user by revoking refresh token.
    /// Prevents further token refreshes with that token.
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    Task<AuthResult> LogoutAsync(string refreshToken);

    /// <summary>
    /// Changes user password (requires current password verification).
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="currentPassword">Current password (must be valid)</param>
    /// <param name="newPassword">New password (must pass policy)</param>
    /// <returns>AuthResult with success/error details</returns>
    Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    /// <summary>
    /// Verifies user exists and returns their details (for internal use only).
    /// </summary>
    /// <param name="userCode">User identifier (Usercode)</param>
    /// <returns>User details, or null if not found</returns>
    Task<AuthUserDto?> GetUserAsync(string userCode);
}

/// <summary>
/// Response DTO for authentication operations (register, login, refresh, logout).
/// Follows standard auth response pattern: Success flag + Data + Errors.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// True if operation succeeded, false otherwise.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Detailed error messages if operation failed.
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Authentication data (tokens, user info) on successful login/register.
    /// Null if operation failed.
    /// </summary>
    public AuthResultData? Data { get; set; }

    /// <summary>
    /// Descriptive message for client (user-friendly error or success message).
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Payload returned on successful authentication (login, register, refresh).
/// Contains tokens and non-sensitive user information.
/// </summary>
public class AuthResultData
{
    /// <summary>
    /// Short-lived JWT access token (15 minutes).
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Long-lived refresh token (7 days).
    /// </summary>
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Token type (always "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token expiration time in seconds.
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// User identifier (Usercode).
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// User roles (for RBAC, e.g., ["Customer", "Admin"]).
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = [];
}

/// <summary>
/// Public user DTO (no sensitive data like password hash).
/// Used for profile endpoints and user lookups.
/// </summary>
public class AuthUserDto
{
    /// <summary>
    /// User identifier (Usercode).
    /// </summary>
    public string Usercode { get; set; } = null!;

    /// <summary>
    /// User email address.
    /// </summary>
    public string UserEmail { get; set; } = null!;

    /// <summary>
    /// Account creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last modification timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User roles.
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = [];

    /// <summary>
    /// Is account currently locked (after failed login attempts).
    /// </summary>
    public bool IsLocked { get; set; }
}
