using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Application.Common.Abstractions;

namespace RestaurantAPI.Infrastructure.Services;

/// <summary>
/// Implementation of IPasswordService using ASP.NET Core Identity's PasswordHasher.
/// Provides strong password hashing, verification, and policy validation.
/// Enforces: minimum 12 characters, no leading/trailing whitespace, not all same character.
///
/// Uses Identity's PasswordVerificationResult directly — no custom enum or conversion needed.
/// NeedsRehashing removed: the PasswordHasher returns SuccessRehashNeeded automatically
/// on VerifyPassword when the hash format is outdated.
/// </summary>
public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher;
    private readonly ILogger<PasswordService> _logger;

    /// <summary>Minimum password length (NIST 800-63B recommendation).</summary>
    private const int MinPasswordLength = 12;

    /// <summary>Maximum password length to prevent DoS attacks.</summary>
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
            return _passwordHasher.HashPassword(null!, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hashing password");
            throw;
        }
    }

    public PasswordVerificationResult VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return PasswordVerificationResult.Failed;

        try
        {
            // Identity's PasswordHasher returns the canonical PasswordVerificationResult directly.
            // No conversion needed — the interface now uses this type.
            return _passwordHasher.VerifyHashedPassword(null!, hash, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password");
            return PasswordVerificationResult.Failed;
        }
    }

    public (bool IsValid, List<string> Errors) ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return (false, errors);
        }

        var trimmed = password.Trim();

        if (trimmed.Length < MinPasswordLength)
            errors.Add($"Password must be at least {MinPasswordLength} characters long");

        if (trimmed.Length > MaxPasswordLength)
            errors.Add($"Password cannot exceed {MaxPasswordLength} characters");

        if (trimmed.Distinct().Count() == 1)
            errors.Add("Password cannot contain only the same character repeated");

        return (errors.Count == 0, errors);
    }
}
