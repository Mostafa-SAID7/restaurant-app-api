using RestaurantAPI.Filters;
using RestaurantAPI.Helpers;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("User management and authentication endpoints")]
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
        /// Register a new user account
        /// </summary>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register new user", Description = "Create a new user account with email and password")]
        [SwaggerResponse(201, "User registered successfully", typeof(User))]
        [SwaggerResponse(400, "Invalid input or validation error")]
        [SwaggerResponse(409, "User already exists")]
        public async Task<ActionResult> RegisterAsync([FromBody] UserDTO userDTO)
        {
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
                usercode = user.Usercode,
                email = user.UserEmail
            });
        }

        /// <summary>
        /// Authenticate user and retrieve API key
        /// </summary>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "User login", Description = "Authenticate user with email and password, returns API key")]
        [SwaggerResponse(200, "Authentication successful")]
        [SwaggerResponse(400, "Invalid email or password")]
        [SwaggerResponse(401, "Authentication failed")]
        [RateLimit(maxRequests: 10, timeWindowMinutes: 1)] // Rate limit login attempts
        public async Task<ActionResult> LoginAsync([FromBody] UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.UserEmail) || string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return ResponseHelper.Error("Email and password are required");
            }

            var userCode = await _userService.GetUserCodeAsync(userDTO.UserEmail, userDTO.Password);
            
            if (userCode != null)
            {
                return ResponseHelper.Success(new { 
                    apikey = userCode,
                    email = userDTO.UserEmail
                }, "Login successful");
            }
            
            _logger.LogWarning($"Failed login attempt for email: {userDTO.UserEmail}");
            return ResponseHelper.Unauthorized("Invalid credentials");
        }

        /// <summary>
        /// Delete user account (requires authentication)
        /// </summary>
        [HttpDelete("account")]
        [SwaggerOperation(Summary = "Delete user account", Description = "Permanently delete user account")]
        [SwaggerResponse(200, "User deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - invalid or missing API key")]
        [SwaggerResponse(404, "User not found")]
        [RequireApiKey]
        public async Task<ActionResult> DeleteAccountAsync()
        {
            var apiKey = HttpContext.Items["ApiKey"]?.ToString();
            if (string.IsNullOrEmpty(apiKey))
            {
                return ResponseHelper.Unauthorized("API key is required");
            }

            var deleted = await _userService.DeleteUserAsync(apiKey);
            
            if (deleted)
            {
                return ResponseHelper.Success<object>(null, "User account deleted successfully");
            }
            
            return ResponseHelper.NotFound("User");
        }

        /// <summary>
        /// Update user password (requires authentication)
        /// </summary>
        [HttpPut("password")]
        [SwaggerOperation(Summary = "Update user password", Description = "Change user's password")]
        [SwaggerResponse(200, "Password updated successfully")]
        [SwaggerResponse(400, "Invalid password")]
        [SwaggerResponse(401, "Unauthorized - invalid or missing API key")]
        [SwaggerResponse(404, "User not found")]
        [RequireApiKey]
        [LogRequests]
        public async Task<ActionResult> UpdatePasswordAsync([FromBody] PasswordUpdateDTO passwordUpdate)
        {
            if (string.IsNullOrEmpty(passwordUpdate?.NewPassword))
            {
                return ResponseHelper.Error("New password is required");
            }

            // Validate password
            var (isValid, errors) = ValidationHelper.ValidatePassword(passwordUpdate.NewPassword);
            if (!isValid)
            {
                return ResponseHelper.ValidationError(errors);
            }

            var apiKey = HttpContext.Items["ApiKey"]?.ToString();
            if (string.IsNullOrEmpty(apiKey))
            {
                return ResponseHelper.Unauthorized("API key is required");
            }

            var updatedUser = await _userService.UpdateUserPasswordAsync(apiKey, passwordUpdate.NewPassword);
            
            if (updatedUser != null)
            {
                return ResponseHelper.Success<object>(null, "Password updated successfully");
            }
            
            return ResponseHelper.NotFound("User");
        }
    }
}