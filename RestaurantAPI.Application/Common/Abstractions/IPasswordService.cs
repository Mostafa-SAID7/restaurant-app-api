namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Result of a password verification operation.
/// Kept in the Application layer (not using Identity's enum directly) to preserve
/// clean architecture — the Application layer must not depend on ASP.NET Core Identity.
/// </summary>
public enum PasswordVerificationResult
{
    /// <summary>Password verification succeeded.</summary>
    Success = 0,

    /// <summary>Password hash is invalid or corrupted.</summary>
    InvalidHash = 1,

    /// <summary>Password verification failed (wrong password).</summary>
    Failed = 2,

    /// <summary>Password verified but hash needs rehashing (upgraded algorithm).</summary>
    SuccessRehashNeeded = 3
}

/// <summary>
/// Service for password hashing, verification, and policy validation.
/// Enforces strong password requirements and handles secure password operations.
/// Uses a custom PasswordVerificationResult enum so the Application layer
/// stays free of any ASP.NET Core Identity dependency.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes a plaintext password using bcrypt.
    /// Result is safe to store in database.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plaintext password against a stored hash.
    /// Returns Success, SuccessRehashNeeded, InvalidHash, or Failed.
    /// </summary>
    PasswordVerificationResult VerifyPassword(string password, string hash);

    /// <summary>
    /// Validates password against the password policy.
    /// Enforces: minimum 12 characters, not whitespace-only, not all same character.
    /// </summary>
    (bool IsValid, List<string> Errors) ValidatePassword(string password);
}
