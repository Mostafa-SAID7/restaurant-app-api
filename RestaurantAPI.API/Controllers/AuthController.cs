using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Features.Auth.Commands;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace RestaurantAPI.API.Controllers;

/// <summary>
/// Authentication controller for user registration, login, token refresh, and logout.
/// Uses CQRS Commands via MediatR for all authentication operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Authentication - Register, Login, Token Management")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account with email and password.
    /// Password must meet policy: minimum 12 characters.
    /// Returns JWT access token and refresh token on success.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register new user account", Description = "Create a new user with email and password")]
    [SwaggerResponse(201, "User registered successfully", typeof(ApiResponse<TokenResponseDto>))]
    [SwaggerResponse(400, "Validation error or user already exists")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RegisterAsync(
        [FromBody] RegisterRequestDto registerDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<TokenResponseDto>.CreateValidationError(errors));
            }

            var command = new RegisterCommand { Email = registerDto.Email, Password = registerDto.Password };
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User registered: {Email}", registerDto.Email);

            return CreatedAtAction(nameof(RegisterAsync), 
                ApiResponse<TokenResponseDto>.CreateSuccess(result, "User registered successfully"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Registration validation failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<TokenResponseDto>.CreateError(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<TokenResponseDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterAsync");
            return StatusCode(500, ApiResponse<TokenResponseDto>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Authenticates user with email and password.
    /// Returns JWT access token (15 min) and refresh token (7 days).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Authenticate user", Description = "Login with email and password to receive JWT tokens")]
    [SwaggerResponse(200, "Login successful", typeof(ApiResponse<TokenResponseDto>))]
    [SwaggerResponse(401, "Invalid credentials")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> LoginAsync(
        [FromBody] LoginRequestDto loginDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<TokenResponseDto>.CreateValidationError(errors));
            }

            var command = new LoginCommand { Email = loginDto.Email, Password = loginDto.Password };
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User logged in: {Email}", loginDto.Email);

            return Ok(ApiResponse<TokenResponseDto>.CreateSuccess(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed: {Message}", ex.Message);
            return Unauthorized(ApiResponse<TokenResponseDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAsync");
            return StatusCode(500, ApiResponse<TokenResponseDto>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Refreshes JWT access token using a valid refresh token.
    /// Implements refresh token rotation: old token invalidated, new pair issued.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refresh access token", Description = "Use refresh token to get new access token")]
    [SwaggerResponse(200, "Token refreshed successfully", typeof(ApiResponse<TokenResponseDto>))]
    [SwaggerResponse(400, "Invalid or expired refresh token")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshAsync(
        [FromBody] RefreshTokenRequestDto refreshDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<TokenResponseDto>.CreateValidationError(errors));
            }

            var command = new RefreshTokenCommand { RefreshToken = refreshDto.RefreshToken };
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Token refreshed for user: {UserId}", result.UserId);

            return Ok(ApiResponse<TokenResponseDto>.CreateSuccess(result, "Token refreshed successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Refresh failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<TokenResponseDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshAsync");
            return StatusCode(500, ApiResponse<TokenResponseDto>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Logs out user by revoking their refresh token.
    /// Prevents future token refreshes with that token.
    /// Requires authentication.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(Summary = "Logout user", Description = "Revoke refresh token and end session")]
    [SwaggerResponse(200, "Logout successful")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<ApiResponse<object>>> LogoutAsync(
        [FromBody] RefreshTokenRequestDto refreshDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<object>.CreateValidationError(errors));
            }

            var command = new LogoutCommand { RefreshToken = refreshDto.RefreshToken };
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User logged out");

            return Ok(ApiResponse<object>.CreateSuccess(null, "Logout successful"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Logout failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LogoutAsync");
            return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
        }
    }

    /// <summary>
    /// Changes user password.
    /// Requires authentication and current password verification.
    /// New password must pass policy validation.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [SwaggerOperation(Summary = "Change user password", Description = "Update password (requires current password verification)")]
    [SwaggerResponse(200, "Password changed successfully")]
    [SwaggerResponse(400, "Invalid current password or new password policy violation")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePasswordAsync(
        [FromBody] ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<object>.CreateValidationError(errors));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.CreateError("User not identified"));
            }

            var command = new ChangePasswordCommand 
            { 
                UserId = userId,
                CurrentPassword = changePasswordDto.CurrentPassword,
                NewPassword = changePasswordDto.NewPassword
            };
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Password changed for user: {UserId}", userId);

            return Ok(ApiResponse<object>.CreateSuccess(null, "Password changed successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Password change failed: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChangePasswordAsync");
            return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
        }
    }
}
