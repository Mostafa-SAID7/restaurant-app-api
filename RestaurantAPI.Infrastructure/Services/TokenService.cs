using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Services;

/// <summary>
/// Implementation of ITokenService.
/// Generates JWT access tokens and refresh tokens.
/// Implements refresh token rotation: old token marked used/revoked, new pair issued.
/// Access tokens are short-lived (15 min); refresh tokens long-lived (7 days).
///
/// Uses IUnitOfWork (not AppDbContext directly) to stay consistent with the
/// repository pattern used throughout the rest of the application.
/// Returns TokenResponseDto (the canonical DTO) fully populated, including
/// UserId, Email and Roles — so CQRS handlers need no additional mapping.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TokenService> _logger;
    private readonly IPasswordService _passwordService;

    /// <summary>
    /// Access token expiration in minutes (15 minutes = short-lived).
    /// </summary>
    private const int AccessTokenExpirationMinutes = 15;

    /// <summary>
    /// Refresh token expiration in days (7 days).
    /// </summary>
    private const int RefreshTokenExpirationDays = 7;

    public TokenService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TokenService> logger,
        IPasswordService passwordService)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _passwordService = passwordService;
    }

    public async Task<TokenResponseDto> GenerateTokensAsync(string userId, string userEmail, IEnumerable<string> roles)
    {
        try
        {
            var roleList = roles.ToList();
            var accessToken = GenerateAccessToken(userId, userEmail, roleList);
            var refreshToken = await GenerateRefreshTokenAsync(userId);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = AccessTokenExpirationMinutes * 60,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpirationMinutes).ToUnixTimeSeconds(),
                UserId = userId,
                Email = userEmail,
                Roles = roleList
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tokens for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TokenResponseDto?> RefreshAccessTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token is null or empty");
                return null;
            }

            // Find refresh token in database via repository
            var allTokens = await _unitOfWork.RefreshTokens.GetAllAsync();
            RefreshToken? token = null;

            // Since we store hashed tokens, we need to verify against each hash
            foreach (var storedToken in allTokens)
            {
                if (_passwordService.VerifyPassword(refreshToken, storedToken.TokenHash) == PasswordVerificationResult.Success)
                {
                    token = storedToken;
                    break;
                }
            }

            if (token == null)
            {
                _logger.LogWarning("Refresh token not found in database");
                return null;
            }

            // Validate token status
            if (token.IsRevoked)
            {
                _logger.LogWarning("Refresh token has been revoked");
                return null;
            }

            if (token.IsExpired)
            {
                _logger.LogWarning("Refresh token has expired");
                return null;
            }

            if (token.UsedAt.HasValue)
            {
                _logger.LogWarning("Refresh token has already been used (possible token reuse attack)");
                // Implement token rotation: revoke all tokens for this user as security measure
                await RevokeAllUserTokensAsync(token.UserId);
                return null;
            }

            // Mark old token as used (rotation)
            token.UsedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshTokens.UpdateAsync(token);
            await _unitOfWork.SaveChangesAsync();

            // Get user and roles
            var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
            if (user == null)
            {
                _logger.LogError("User {UserId} not found", token.UserId);
                return null;
            }

            var userRoles = await _unitOfWork.UserRoles.GetUserRolesWithDetailsAsync(token.UserId);
            var roles = userRoles
                .Select(ur => ur.Role?.Name)
                .Where(name => name != null)
                .Cast<string>()
                .ToList();

            // Generate new token pair (fully populated DTO)
            var newTokens = await GenerateTokensAsync(token.UserId, user.UserEmail, roles);

            _logger.LogInformation("Refresh token used successfully for user {UserId}", token.UserId);
            return newTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing access token");
            return null;
        }
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
                return;

            var allTokens = await _unitOfWork.RefreshTokens.GetAllAsync();

            foreach (var storedToken in allTokens)
            {
                if (_passwordService.VerifyPassword(refreshToken, storedToken.TokenHash) == PasswordVerificationResult.Success)
                {
                    storedToken.RevokedAt = DateTime.UtcNow;
                    await _unitOfWork.RefreshTokens.UpdateAsync(storedToken);
                    break;
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
        }
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured"));

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken _);

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    /// <summary>
    /// Generates JWT access token with claims for user identification and authorization.
    /// </summary>
    private string GenerateAccessToken(string userId, string userEmail, IEnumerable<string> roles)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured"));
        var issuer = _configuration["Jwt:Issuer"] ?? "RestaurantAPI";
        var audience = _configuration["Jwt:Audience"] ?? "RestaurantAPIClients";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, userEmail),
            new("jti", Guid.NewGuid().ToString()),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates hashed refresh token and stores in database via IUnitOfWork.
    /// </summary>
    private async Task<string> GenerateRefreshTokenAsync(string userId)
    {
        var randomToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        var tokenHash = _passwordService.HashPassword(randomToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetClientIpAddress()
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return randomToken;
    }

    /// <summary>
    /// Revokes all refresh tokens for a user (security measure for detected token reuse).
    /// </summary>
    private async Task RevokeAllUserTokensAsync(string userId)
    {
        var tokens = await _unitOfWork.RefreshTokens.FindAsync(t => t.UserId == userId && t.RevokedAt == null);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshTokens.UpdateAsync(token);
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogWarning("All refresh tokens revoked for user {UserId} (security measure)", userId);
    }

    /// <summary>
    /// Gets the client IP address for audit trail using IHttpContextAccessor.
    /// Checks X-Forwarded-For, X-Real-IP, then falls back to RemoteIpAddress.
    /// </summary>
    private string? GetClientIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return null;

        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
            return forwarded.Split(',')[0].Trim();

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
