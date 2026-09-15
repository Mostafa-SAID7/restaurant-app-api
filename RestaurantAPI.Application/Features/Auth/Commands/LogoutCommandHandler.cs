using MediatR;
using RestaurantAPI.Application.Services;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for LogoutCommand.
/// Revokes refresh token to end user session.
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken);
        if (!result.Success)
            throw new InvalidOperationException(result.Message);

        return true;
    }
}
