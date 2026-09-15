using Microsoft.AspNetCore.Identity;

namespace RestaurantAPI.Auth.Services.Interfaces;

/// <summary>
/// Service for password hashing, verification, and policy validation.
/// Enforces strong password requirements and handles secure password operations.
/// Implements separation of concerns: password lifecycle management only.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes a plaintext password using bcrypt with configurable cost factor.
    /// Result is safe to store in database.
    /// Never returns plaintext or weak hashes.
    /// </summary>
    /// <param name="password">Plaintext password to hash</param>
    /// <returns>Bcrypt-hashed password string</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plaintext password against a bcrypt hash.
    /// Returns three-state result: Success, InvalidHash (corrupted), Failed (wrong password).
    /// </summary>
    /// <param name="password">Plaintext password to verify</param>
    /// <param name="hash">Bcrypt hash to verify against</param>
    /// <returns>Verification result (Success, InvalidHash, Failed)</returns>
    PasswordVerificationResult VerifyPassword(string password, string hash);

    /// <summary>
    /// Validates password against organization's password policy.
    /// Enforces: minimum length, no spaces, not empty, character distribution rules.
    /// </summary>
    /// <param name="password">Plaintext password to validate</param>
    /// <returns>Validation result (IsValid, Errors list)</returns>
    (bool IsValid, List<string> Errors) ValidatePassword(string password);

    /// <summary>
    /// Checks if password needs rehashing (e.g., bcrypt cost factor increased for security).
    /// Called after verification to support transparent password upgrade.
    /// </summary>
    /// <param name="hash">Bcrypt hash to check</param>
    /// <returns>True if hash should be recreated with current cost factor</returns>
    bool NeedsRehashing(string hash);
}
