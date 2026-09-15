using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Features.Users.Commands;
using RestaurantAPI.Auth.Policies;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("User Management")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Delete user account (requires authentication with JWT)
        /// </summary>
        [HttpDelete("account")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "Delete user account", Description = "Permanently delete user account (requires JWT authentication)")]
        [SwaggerResponse(200, "User deleted successfully", typeof(ApiResponse<object>))]
        [SwaggerResponse(401, "Unauthorized - valid JWT token required")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAccountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.CreateError("User not identified from JWT"));
                }

                var command = new DeleteUserCommand { UserId = userId };
                var result = await _mediator.Send(command, cancellationToken);

                if (!result)
                {
                    return NotFound(ApiResponse<object>.CreateError("User not found"));
                }

                return Ok(ApiResponse<object>.CreateSuccess(null, "User account deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user account");
                return StatusCode(500, ApiResponse<object>.CreateError("An error occurred"));
            }
        }

        // Deprecated endpoints removed in favor of /api/auth endpoints
        // RegisterAsync → POST /api/auth/register
        // LoginAsync → POST /api/auth/login
        // UpdatePasswordAsync → POST /api/auth/change-password
    }
}
