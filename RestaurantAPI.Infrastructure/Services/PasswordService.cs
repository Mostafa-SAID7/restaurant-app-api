using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Application.Common.Abstractions;

namespace RestaurantAPI.Infrastructure.Services;

/// <summary>
/// Implementation of IPasswordService using ASP.NET Core Identity's PasswordHasher.
/// Maps Identity's PasswordVerificationResult to the Application layer's custom enum
/// so the Application layer stays free of any ASP.NET Core Identity dependency.
/// </summary>
public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher;
    private readonly ILogger<PasswordService> _logger;

    private const int MinPasswordLength = 12;
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

    public RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult.Failed;

        try
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hash, password);

            // Map Identity enum → Application layer custom enum (preserves clean architecture)
            return result switch
            {
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success
                    => RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult.Success,
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded
                    => RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult.SuccessRehashNeeded,
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed
                    => RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult.Failed,
                _ => RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password");
            return RestaurantAPI.Application.Common.Abstractions.PasswordVerificationResult.Failed;
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
