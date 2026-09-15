using Microsoft.AspNetCore.Identity;

namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Service for password hashing, verification, and policy validation.
/// Enforces strong password requirements and handles secure password operations.
/// Implements separation of concerns: password lifecycle management only.
///
/// Uses Microsoft.AspNetCore.Identity.PasswordVerificationResult directly —
/// no custom enum wrapper needed, which eliminates the conversion switch in the implementation.
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
    /// Returns the Identity PasswordVerificationResult: Success, SuccessRehashNeeded, or Failed.
    /// </summary>
    /// <param name="password">Plaintext password to verify</param>
    /// <param name="hash">Bcrypt hash to verify against</param>
    /// <returns>Verification result (Success, SuccessRehashNeeded, Failed)</returns>
    PasswordVerificationResult VerifyPassword(string password, string hash);

    /// <summary>
    /// Validates password against organization's password policy.
    /// Enforces: minimum length, not empty, not whitespace-only, not all same character.
    /// </summary>
    /// <param name="password">Plaintext password to validate</param>
    /// <returns>Validation result (IsValid, Errors list)</returns>
    (bool IsValid, List<string> Errors) ValidatePassword(string password);
}
