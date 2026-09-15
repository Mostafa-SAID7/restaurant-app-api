using MediatR;
using RestaurantAPI.Application.Services;

namespace RestaurantAPI.Application.Features.Auth.Commands;

/// <summary>
/// Handler for ChangePasswordCommand.
/// Changes password for authenticated user.
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IAuthService _authService;

    public ChangePasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
        if (!result.Success)
            throw new InvalidOperationException(result.Message);

        return true;
    }
}
