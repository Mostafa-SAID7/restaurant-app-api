using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for LoginCommand.
/// Authenticates user and returns token pair.
/// Implements full login logic: credential verification, role retrieval, token generation.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<TokenResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and password are required");

            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            var verificationResult = _passwordService.VerifyPassword(request.Password, user.PasswordHash);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetUserRolesWithDetailsAsync(user.Usercode);
            var roles = userRoles.Select(ur => ur.Role?.Name).Where(name => name != null).Cast<string>().ToList();

            // Generate tokens (service now populates UserId, Email, Roles)
            var tokenResponse = await _tokenService.GenerateTokensAsync(
                user.Usercode,
                user.UserEmail,
                roles);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw;
        }
    }
}
