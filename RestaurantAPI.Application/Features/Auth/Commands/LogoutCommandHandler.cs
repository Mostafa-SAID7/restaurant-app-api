using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for LogoutCommand.
/// Revokes refresh token to end user session.
/// Implements logout logic: token revocation via TokenService.
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        ITokenService tokenService,
        ILogger<LogoutCommandHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new ArgumentException("Refresh token is required");

            await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);

            _logger.LogInformation("User logged out successfully (token revoked)");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            throw;
        }
    }
}
