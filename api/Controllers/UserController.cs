using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Auth.Policies;
using RestaurantAPI.Filters;
using RestaurantAPI.Helpers;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("User Management (Deprecated - use /api/auth)")]
    [Obsolete("Use /api/auth endpoints for authentication and /api/profile for profile management. This controller will be removed in v2.0")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Register a new user account (DEPRECATED - use POST /api/auth/register)
        /// </summary>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "[DEPRECATED] Register new user", Description = "DEPRECATED: Use POST /api/auth/register instead")]
        [SwaggerResponse(201, "User registered successfully")]
        [SwaggerResponse(400, "Invalid input or validation error")]
        [SwaggerResponse(409, "User already exists")]
        public async Task<ActionResult> RegisterAsync([FromBody] UserDTO userDTO)
        {
            _logger.LogWarning("Deprecated endpoint /api/user/register called. Use /api/auth/register instead");

            // Validate input
            var (isValid, errors) = ValidationHelper.ValidateUserRegistration(userDTO.UserEmail, userDTO.Password);
            if (!isValid)
            {
                return ResponseHelper.ValidationError(errors);
            }

            // Check if user exists
            var userExists = await _userService.UserExistsAsync(userDTO.UserEmail);
            if (userExists)
            {
                return ResponseHelper.Error("User already exists", 409);
            }

            var user = await _userService.RegisterUserAsync(userDTO);
            return ResponseHelper.Created(new { 
                email = user.UserEmail,
                message = "DEPRECATED: Use /api/auth/register for JWT tokens"
            });
        }

        /// <summary>
        /// Authenticate user and retrieve API key (DEPRECATED - use POST /api/auth/login)
        /// </summary>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "[DEPRECATED] User login", Description = "DEPRECATED: Use POST /api/auth/login instead")]
        [SwaggerResponse(200, "Authentication successful")]
        [SwaggerResponse(400, "Invalid email or password")]
        [SwaggerResponse(401, "Authentication failed")]
        [RateLimit(maxRequests: 10, timeWindowMinutes: 1)]
        public async Task<ActionResult> LoginAsync([FromBody] UserDTO userDTO)
        {
            _logger.LogWarning("Deprecated endpoint /api/user/login called. Use /api/auth/login instead");

            if (string.IsNullOrWhiteSpace(userDTO.UserEmail) || string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return ResponseHelper.Error("Email and password are required");
            }

            // API-key authentication removed (Phase A.1)
            // All authentication now goes through /api/auth with JWT
            return ResponseHelper.Error("This endpoint is deprecated and will be removed in v2.0. Use POST /api/auth/login instead", 410); // 410 Gone
        }

        /// <summary>
        /// Delete user account (requires authentication with JWT)
        /// </summary>
        [HttpDelete("account")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "Delete user account", Description = "Permanently delete user account (requires JWT authentication)")]
        [SwaggerResponse(200, "User deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - valid JWT token required")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> DeleteAccountAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseHelper.Unauthorized("User not identified from JWT");
            }

            var deleted = await _userService.DeleteUserAsync(userId);
            
            if (deleted)
            {
                return ResponseHelper.Success<object>(null, "User account deleted successfully");
            }
            
            return ResponseHelper.NotFound("User");
        }

        /// <summary>
        /// Update user password (DEPRECATED - use POST /api/auth/change-password)
        /// </summary>
        [HttpPut("password")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "[DEPRECATED] Update user password", Description = "DEPRECATED: Use POST /api/auth/change-password instead")]
        [SwaggerResponse(200, "Password updated successfully")]
        [SwaggerResponse(400, "Invalid password")]
        [SwaggerResponse(401, "Unauthorized - valid JWT token required")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(410, "Endpoint deprecated")]
        [LogRequests]
        public async Task<ActionResult> UpdatePasswordAsync([FromBody] PasswordUpdateDTO passwordUpdate)
        {
            _logger.LogWarning("Deprecated endpoint /api/user/password called. Use /api/auth/change-password instead");
            
            // Password changes now go through /api/auth/change-password (Phase A.1)
            return ResponseHelper.Error("This endpoint is deprecated and will be removed in v2.0. Use POST /api/auth/change-password instead", 410); // 410 Gone
        }
    }
}
