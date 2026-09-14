using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Auth.Models;
using RestaurantAPI.Auth.Services.Interfaces;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;

namespace RestaurantAPI.Auth.Services.Implementation;

/// <summary>
/// Implementation of IAuthService.
/// Orchestrates authentication workflows: register, login, refresh, logout, password change.
/// Phase A.6: Depends on IUnitOfWork (repositories) instead of AppDbContext (concrete)
/// Single Responsibility: coordinate PasswordService and TokenService only.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Maximum failed login attempts before account lockout (if implemented).
    /// </summary>
    private const int MaxFailedLoginAttempts = 5;

    /// <summary>
    /// Account lockout duration in minutes.
    /// </summary>
    private const int LockoutDurationMinutes = 15;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Email and password are required",
                    Errors = ["Email and password must not be empty"]
                };
            }

            // Validate password policy
            var (isPasswordValid, passwordErrors) = _passwordService.ValidatePassword(password);
            if (!isPasswordValid)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Password does not meet policy requirements",
                    Errors = passwordErrors
                };
            }

            // Check if email already exists
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", email);
                return new AuthResult
                {
                    Success = false,
                    Message = "User already exists",
                    Errors = ["Email is already registered"]
                };
            }

            // Create new user
            var usercode = Guid.NewGuid().ToString();
            var user = new User
            {
                Usercode = usercode,
                UserEmail = email,
                PasswordHash = _passwordService.HashPassword(password),
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

            // Generate tokens
            var tokens = await _tokenService.GenerateTokensAsync(
                usercode,
                email,
                new[] { "Customer" });

            _logger.LogInformation("User registered successfully: {Email}", email);

            return new AuthResult
            {
                Success = true,
                Message = "User registered successfully",
                Data = new AuthResultData
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                    TokenType = tokens.TokenType,
                    ExpiresIn = tokens.ExpiresIn,
                    UserId = usercode,
                    Email = email,
                    Roles = new[] { "Customer" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return new AuthResult
            {
                Success = false,
                Message = "Registration failed",
                Errors = ["An unexpected error occurred during registration"]
            };
        }
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = ["Email and password are required"]
                };
            }

            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = ["Invalid email or password"]
                };
            }

            // Verify password
            var verificationResult = _passwordService.VerifyPassword(password, user.PasswordHash);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", email);
                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = ["Invalid email or password"]
                };
            }

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.GetUserRolesWithDetailsAsync(user.Usercode);
            var roles = userRoles.Select(ur => ur.Role?.Name).Where(name => name != null).Cast<string>().ToList();

            // Generate tokens
            var tokens = await _tokenService.GenerateTokensAsync(
                user.Usercode,
                user.UserEmail,
                roles);

            _logger.LogInformation("User logged in successfully: {Email}", email);

            return new AuthResult
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthResultData
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                    TokenType = tokens.TokenType,
                    ExpiresIn = tokens.ExpiresIn,
                    UserId = user.Usercode,
                    Email = user.UserEmail,
                    Roles = roles
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return new AuthResult
            {
                Success = false,
                Message = "Login failed",
                Errors = ["An unexpected error occurred during login"]
            };
        }
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Refresh token is required",
                Errors = ["Refresh token must not be empty"]
            };
        }

        var tokens = await _tokenService.RefreshAccessTokenAsync(refreshToken);
        if (tokens == null)
        {
            _logger.LogWarning("Failed to refresh token (invalid or expired)");
            return new AuthResult
            {
                Success = false,
                Message = "Refresh failed",
                Errors = ["Refresh token is invalid, expired, or has been revoked"]
            };
        }

        // Get the stored token by value
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
        if (token == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Refresh failed",
                Errors = ["Token not found"]
            };
        }

        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
        var userRoles = await _unitOfWork.UserRoles.GetUserRolesWithDetailsAsync(token.UserId);
        var roles = userRoles.Select(ur => ur.Role?.Name).Where(name => name != null).Cast<string>().ToList();

        return new AuthResult
        {
            Success = true,
            Message = "Token refreshed successfully",
            Data = new AuthResultData
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                TokenType = tokens.TokenType,
                ExpiresIn = tokens.ExpiresIn,
                UserId = token.UserId,
                Email = user?.UserEmail ?? "unknown",
                Roles = roles
            }
        };
    }

    public async Task<AuthResult> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Refresh token is required",
                Errors = ["Refresh token must not be empty"]
            };
        }

        await _tokenService.RevokeRefreshTokenAsync(refreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Logged out successfully"
        };
    }

    public async Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "User not found",
                    Errors = ["User ID is required"]
                };
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "User not found",
                    Errors = ["User does not exist"]
                };
            }

            // Verify current password
            var verificationResult = _passwordService.VerifyPassword(currentPassword, user.PasswordHash);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Password change attempt with incorrect current password for user: {UserId}", userId);
                return new AuthResult
                {
                    Success = false,
                    Message = "Current password is incorrect",
                    Errors = ["Current password does not match"]
                };
            }

            // Validate new password
            var (isValid, errors) = _passwordService.ValidatePassword(newPassword);
            if (!isValid)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "New password does not meet policy requirements",
                    Errors = errors
                };
            }

            // Hash and update password
            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);

            return new AuthResult
            {
                Success = true,
                Message = "Password changed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return new AuthResult
            {
                Success = false,
                Message = "Password change failed",
                Errors = ["An unexpected error occurred"]
            };
        }
    }

    public async Task<AuthUserDto?> GetUserAsync(string userCode)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userCode);
        if (user == null)
            return null;

        var userRoles = await _unitOfWork.UserRoles.GetUserRolesWithDetailsAsync(user.Usercode);
        var roles = userRoles.Select(ur => ur.Role?.Name).Where(name => name != null).Cast<string>().ToList();

        return new AuthUserDto
        {
            Usercode = user.Usercode,
            UserEmail = user.UserEmail,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = roles,
            IsLocked = false // TODO: Implement account lockout
        };
    }
}
