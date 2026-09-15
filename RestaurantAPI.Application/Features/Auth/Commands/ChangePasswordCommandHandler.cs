using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for ChangePasswordCommand.
/// Changes password for authenticated user.
/// Implements password change logic: current password verification, policy validation, hashing.
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("User ID is required");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Verify current password
            var verificationResult = _passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Password change attempt with incorrect current password for user: {UserId}", request.UserId);
                throw new UnauthorizedAccessException("Current password does not match");
            }

            // Validate new password
            var (isValid, errors) = _passwordService.ValidatePassword(request.NewPassword);
            if (!isValid)
                throw new ArgumentException($"Password policy violation: {string.Join(", ", errors)}");

            // Hash and update password
            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", request.UserId);
            throw;
        }
    }
}
