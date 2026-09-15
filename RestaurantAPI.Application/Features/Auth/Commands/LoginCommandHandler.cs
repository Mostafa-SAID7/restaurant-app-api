using MediatR;
using RestaurantAPI.Application.Services;
using RestaurantAPI.Auth.DTOs;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for LoginCommand.
/// Authenticates user and returns token pair.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<TokenResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        if (!result.Success)
            throw new UnauthorizedAccessException(result.Message);

        return result.Data!;
    }
}
