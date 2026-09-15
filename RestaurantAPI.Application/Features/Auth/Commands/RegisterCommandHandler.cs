using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Domain.Interfaces;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RegisterCommand.
/// Registers a new user and returns initial token pair.
/// Implements full registration logic: validation, user creation, role assignment, token generation.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<RegisterCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<TokenResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and password are required");

            // Validate password policy
            var (isPasswordValid, passwordErrors) = _passwordService.ValidatePassword(request.Password);
            if (!isPasswordValid)
                throw new ArgumentException($"Password policy violation: {string.Join(", ", passwordErrors)}");

            // Check if email already exists
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
                throw new InvalidOperationException("User already exists");
            }

            // Create new user
            var usercode = Guid.NewGuid().ToString();
            var user = new User
            {
                Usercode = usercode,
                UserEmail = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);

            // Assign default "Customer" role
            var customerRole = await _unitOfWork.Roles.GetByNameAsync("Customer");
            if (customerRole == null)
            {
                // Create Customer role if it doesn't exist
                customerRole = new ApplicationRole
                {
                    Name = "Customer",
                    Description = "Standard user role"
                };
                await _unitOfWork.Roles.AddAsync(customerRole);
            }

            var userRole = new ApplicationUserRole
            {
                UserId = usercode,
                RoleId = customerRole.Id,
                AssignedAt = DateTime.UtcNow
            };
            await _unitOfWork.UserRoles.AddAsync(userRole);

            await _unitOfWork.SaveChangesAsync();

            // Generate tokens (service now populates UserId, Email, Roles)
            var tokenResponse = await _tokenService.GenerateTokensAsync(
                usercode,
                request.Email,
                new[] { "Customer" });

            _logger.LogInformation("User registered successfully: {Email}", request.Email);

            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            throw;
        }
    }
}
