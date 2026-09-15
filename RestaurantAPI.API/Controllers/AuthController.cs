using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Auth.DTOs;
using RestaurantAPI.Auth.Services.Interfaces;
using RestaurantAPI.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using RestaurantAPI.Auth.Services.Interfaces;

namespace RestaurantAPI.Auth.Controllers;

/// <summary>
/// Authentication controller for user registration, login, token refresh, and logout.
/// Phase A.4: Uses unified ApiResponse<T> contract for all responses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Authentication - Register, Login, Token Management")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account with email and password.
    /// Password must meet policy: minimum 12 characters.
    /// Returns JWT access token and refresh token on success.
    /// </summary>
    /// <param name="registerDto">Registration credentials</param>
    /// <returns>Token response with user details</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register new user account", Description = "Create a new user with email and password")]
    [SwaggerResponse(201, "User registered successfully", typeof(ApiResponse<AuthResultData>))]
    [SwaggerResponse(400, "Validation error or user already exists")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequestDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                var response = ApiResponse<object>.CreateValidationError(errors);
                return BadRequest(response);
            }

            var result = await _authService.RegisterAsync(registerDto.Email, registerDto.Password);

            if (!result.Success)
            {
                var response = ApiResponse<object>.CreateError(result.Message);
                response.Errors = result.Errors;
                return BadRequest(response);
            }

            _logger.LogInformation("User registered: {Email}", registerDto.Email);

            var successResponse = ApiResponse<AuthResultData>.CreateSuccess(result.Data, result.Message);
            return CreatedAtAction(nameof(RegisterAsync), successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterAsync");
            var response = ApiResponse<object>.CreateError("Internal server error");
            response.Errors = new List<string> { "An unexpected error occurred" };
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Authenticates user with email and password.
    /// Returns JWT access token (15 min) and refresh token (7 days).
    /// Rate-limited to 10 requests per minute.
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Token response with user details</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Authenticate user", Description = "Login with email and password to receive JWT tokens")]
    [SwaggerResponse(200, "Login successful", typeof(ApiResponse<TokenResponseDto>))]
    [SwaggerResponse(400, "Invalid credentials")]
    [SwaggerResponse(429, "Rate limit exceeded")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginRequestDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                var response = ApiResponse<object>.CreateValidationError(errors);
                return BadRequest(response);
            }

            var result = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (!result.Success)
            {
                // Don't expose whether email exists (security best practice)
                var response = ApiResponse<object>.CreateError(result.Message);
                response.Errors = result.Errors;
                return Unauthorized(response);
            }

            _logger.LogInformation("User logged in: {Email}", loginDto.Email);

            var successResponse = ApiResponse<AuthResultData>.CreateSuccess(result.Data, result.Message);
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAsync");
            var response = ApiResponse<object>.CreateError("Internal server error");
            response.Errors = new List<string> { "An unexpected error occurred" };
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Refreshes JWT access token using a valid refresh token.
    /// Implements refresh token rotation: old token invalidated, new pair issued.
    /// No rate limiting on refresh (prevents denial of service for valid users).
    /// </summary>
    /// <param name="refreshDto">Refresh token request</param>
    /// <returns>New token response with new token pair</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refresh access token", Description = "Use refresh token to get new access token without re-entering password")]
    [SwaggerResponse(200, "Token refreshed successfully", typeof(ApiResponse<TokenResponseDto>))]
    [SwaggerResponse(400, "Invalid or expired refresh token")]
    public async Task<ActionResult> RefreshAsync([FromBody] RefreshTokenRequestDto refreshDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                var response = ApiResponse<object>.CreateValidationError(errors);
                return BadRequest(response);
            }

            var result = await _authService.RefreshAsync(refreshDto.RefreshToken);

            if (!result.Success)
            {
                var response = ApiResponse<object>.CreateError(result.Message);
                response.Errors = result.Errors;
                return BadRequest(response);
            }

            _logger.LogInformation("Token refreshed for user: {UserId}", result.Data?.UserId);

            var successResponse = ApiResponse<AuthResultData>.CreateSuccess(result.Data, result.Message);
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshAsync");
            var response = ApiResponse<object>.CreateError("Internal server error");
            response.Errors = new List<string> { "An unexpected error occurred" };
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Logs out user by revoking their refresh token.
    /// Prevents future token refreshes with that token.
    /// Requires authentication; user must provide their refresh token.
    /// </summary>
    /// <param name="refreshDto">Refresh token to revoke</param>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(Summary = "Logout user", Description = "Revoke refresh token and end session")]
    [SwaggerResponse(200, "Logout successful")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult> LogoutAsync([FromBody] RefreshTokenRequestDto refreshDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                var response = ApiResponse<object>.CreateValidationError(errors);
                return BadRequest(response);
            }

            var result = await _authService.LogoutAsync(refreshDto.RefreshToken);

            if (!result.Success)
            {
                var response = ApiResponse<object>.CreateError(result.Message);
                response.Errors = result.Errors;
                return BadRequest(response);
            }

            _logger.LogInformation("User logged out");

            var successResponse = ApiResponse<object>.CreateSuccess(null, result.Message);
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LogoutAsync");
            var response = ApiResponse<object>.CreateError("Internal server error");
            response.Errors = new List<string> { "An unexpected error occurred" };
            return StatusCode(500, response);
        }
    }

    /// <summary>
    /// Changes user password.
    /// Requires authentication; user must provide current password and new password.
    /// New password must pass policy validation.
    /// </summary>
    /// <param name="changePasswordDto">Current password + new password</param>
    /// <returns>Success response</returns>
    [HttpPost("change-password")]
    [Authorize]
    [SwaggerOperation(Summary = "Change user password", Description = "Update password (requires current password verification)")]
    [SwaggerResponse(200, "Password changed successfully")]
    [SwaggerResponse(400, "Invalid current password or new password policy violation")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                var response = ApiResponse<object>.CreateValidationError(errors);
                return BadRequest(response);
            }

            // Get current user ID from JWT
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                var response = ApiResponse<object>.CreateError("User not identified");
                return Unauthorized(response);
            }

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Success)
            {
                var response = ApiResponse<object>.CreateError(result.Message);
                response.Errors = result.Errors;
                return BadRequest(response);
            }

            _logger.LogInformation("Password changed for user: {UserId}", userId);

            var successResponse = ApiResponse<object>.CreateSuccess(null, result.Message);
            return Ok(successResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChangePasswordAsync");
            var response = ApiResponse<object>.CreateError("Internal server error");
            response.Errors = new List<string> { "An unexpected error occurred" };
            return StatusCode(500, response);
        }
    }
}
