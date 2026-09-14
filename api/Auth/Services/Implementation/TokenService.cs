using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Auth.Models;
using RestaurantAPI.Auth.Services.Interfaces;
using RestaurantAPI.Data;

namespace RestaurantAPI.Auth.Services.Implementation;

/// <summary>
/// Implementation of ITokenService.
/// Generates JWT access tokens and refresh tokens.
/// Implements refresh token rotation: old token marked used/revoked, new pair issued.
/// Access tokens are short-lived (15 min); refresh tokens long-lived (7 days).
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
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
        AppDbContext context,
        ILogger<TokenService> logger,
        IPasswordService passwordService)
    {
        _configuration = configuration;
        _context = context;
        _logger = logger;
        _passwordService = passwordService;
    }

    public async Task<TokenResponse> GenerateTokensAsync(string userId, string userEmail, IEnumerable<string> roles)
    {
        try
        {
            var accessToken = GenerateAccessToken(userId, userEmail, roles);
            var refreshToken = await GenerateRefreshTokenAsync(userId);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = AccessTokenExpirationMinutes * 60, // In seconds
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpirationMinutes).ToUnixTimeSeconds()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tokens for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TokenResponse?> RefreshAccessTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token is null or empty");
                return null;
            }

            // Find refresh token in database
            var storedTokens = await _context.RefreshTokens.ToListAsync();
            RefreshToken? token = null;

            // Since we store hashed tokens, we need to verify against each hash
            foreach (var storedToken in storedTokens)
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
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            // Get user to retrieve roles
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Usercode == token.UserId);
            if (user == null)
            {
                _logger.LogError("User {UserId} not found", token.UserId);
                return null;
            }

            // Get user roles (from junction table)
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == token.UserId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            // Generate new token pair
            var newTokens = await GenerateTokensAsync(token.UserId, user.UserEmail, userRoles);

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

            // Find and revoke token
            var storedTokens = await _context.RefreshTokens.ToListAsync();

            foreach (var storedToken in storedTokens)
            {
                if (_passwordService.VerifyPassword(refreshToken, storedToken.TokenHash) == PasswordVerificationResult.Success)
                {
                    storedToken.RevokedAt = DateTime.UtcNow;
                    _context.RefreshTokens.Update(storedToken);
                    break;
                }
            }

            await _context.SaveChangesAsync();
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
            }, out SecurityToken validatedToken);

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
            new("jti", Guid.NewGuid().ToString()), // JWT ID for revocation tracking
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) // Issued at
        };

        // Add role claims
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
    /// Generates hashed refresh token and stores in database.
    /// </summary>
    private async Task<string> GenerateRefreshTokenAsync(string userId)
    {
        // Generate random refresh token (not hashed yet; will be hashed before storing)
        var randomToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

        // Hash token before storing (treat like password)
        var tokenHash = _passwordService.HashPassword(randomToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetClientIpAddress()
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Return unhashed token to client (only once)
        return randomToken;
    }

    /// <summary>
    /// Revokes all refresh tokens for a user (security measure for detected token reuse).
    /// </summary>
    private async Task RevokeAllUserTokensAsync(string userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Update(token);
        }

        await _context.SaveChangesAsync();
        _logger.LogWarning("All refresh tokens revoked for user {UserId} (security measure)", userId);
    }

    /// <summary>
    /// Gets client IP address for audit trail.
    /// </summary>
    private string? GetClientIpAddress()
    {
        // This would require IHttpContextAccessor to be injected if needed.
        // For now, return null; can be enhanced later.
        return null;
    }
}
