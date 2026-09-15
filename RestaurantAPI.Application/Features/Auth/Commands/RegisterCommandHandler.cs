using MediatR;
using RestaurantAPI.Application.Services;
using RestaurantAPI.Auth.DTOs;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RegisterCommand.
/// Registers a new user and returns initial token pair.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<TokenResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request.Email, request.Password);
        if (!result.Success)
            throw new InvalidOperationException(result.Message);

        return result.Data!;
    }
}
