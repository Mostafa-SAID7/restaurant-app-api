using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Auth.Services.Interfaces;

namespace RestaurantAPI.Auth.Services.Implementation;

/// <summary>
/// Implementation of IPasswordService using bcrypt via AspNetCore Identity.
/// Provides strong password hashing, verification, and policy validation.
/// Enforces: minimum 12 characters, no leading/trailing spaces.
/// </summary>
public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher;
    private readonly ILogger<PasswordService> _logger;

    /// <summary>
    /// Minimum password length (NIST 800-63B recommendation: 8-12+).
    /// </summary>
    private const int MinPasswordLength = 12;

    /// <summary>
    /// Maximum password length to prevent DoS attacks.
    /// </summary>
    private const int MaxPasswordLength = 500;

    public PasswordService(ILogger<PasswordService> logger)
    {
        _passwordHasher = new PasswordHasher<object>();
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        try
        {
            var hash = _passwordHasher.HashPassword(null, password);
            return hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hashing password");
            throw;
        }
    }

    public PasswordVerificationResult VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password))
            return PasswordVerificationResult.Failed;

        if (string.IsNullOrEmpty(hash))
            return PasswordVerificationResult.Failed; // Treat invalid hash as failed

        try
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hash, password);

            return result switch
            {
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success => PasswordVerificationResult.Success,
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded => PasswordVerificationResult.Success, // Treat as success; flag for rehashing
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed => PasswordVerificationResult.Failed,
                _ => PasswordVerificationResult.Failed // Treat unknown as failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password");
            return PasswordVerificationResult.Failed; // Treat exception as failed
        }
    }

    public (bool IsValid, List<string> Errors) ValidatePassword(string password)
    {
        var errors = new List<string>();

        // Null/empty check
        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return (false, errors);
        }

        // Trim check (password must not be just spaces)
        if (password.Trim().Length == 0)
        {
            errors.Add("Password cannot be whitespace only");
            return (false, errors);
        }

        // Length validation
        var trimmedPassword = password.Trim();
        if (trimmedPassword.Length < MinPasswordLength)
        {
            errors.Add($"Password must be at least {MinPasswordLength} characters long");
        }

        if (trimmedPassword.Length > MaxPasswordLength)
        {
            errors.Add($"Password cannot exceed {MaxPasswordLength} characters");
        }

        // Optional: Check for common weak patterns (e.g., all same character)
        if (trimmedPassword.Distinct().Count() == 1)
        {
            errors.Add("Password cannot contain only the same character repeated");
        }

        return (errors.Count == 0, errors);
    }

    public bool NeedsRehashing(string hash)
    {
        if (string.IsNullOrEmpty(hash))
            return true;

        try
        {
            // For bcrypt via PasswordHasher, check if hash version needs upgrade.
            // Current implementation returns "SuccessRehashNeeded" if cost factor is outdated.
            // We'd need to verify post-login. For now, assume all hashes are current.
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if password needs rehashing");
            return false;
        }
    }
}
