using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Application.Common.Abstractions;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RefreshTokenCommand.
/// Refreshes expired access token using refresh token.
/// Implements token refresh logic: validation, rotation, and new token generation.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new ArgumentException("Refresh token is required");

            // Use TokenService to refresh (service returns complete TokenResponseDto)
            var tokenResponse = await _tokenService.RefreshAccessTokenAsync(request.RefreshToken);
            if (tokenResponse == null)
            {
                _logger.LogWarning("Failed to refresh token (invalid or expired)");
                throw new InvalidOperationException("Refresh token is invalid, expired, or has been revoked");
            }

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", tokenResponse.UserId);

            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }
}
