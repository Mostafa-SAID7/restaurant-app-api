using MediatR;
using RestaurantAPI.Application.Services;
using RestaurantAPI.Auth.DTOs;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RefreshTokenCommand.
/// Refreshes expired access token using refresh token.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken);
        if (!result.Success)
            throw new InvalidOperationException(result.Message);

        return result.Data!;
    }
}
