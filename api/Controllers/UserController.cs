using RestaurantAPI.Filters;
using RestaurantAPI.Helpers;
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
            try
            {
                // Validate input
                var (isValid, errors) = ValidationHelper.ValidateUserRegistration(userDTO.UserEmail, userDTO.Password);
                if (!isValid)
                {
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Check if user exists
                var userExists = await _userService.UserExistsAsync(userDTO.UserEmail);
                if (userExists)
                {
                    return StatusCode(409, new { message = "User already exists" });
                }

                var user = await _userService.RegisterUserAsync(userDTO);
                return StatusCode(201, new { 
                    message = "User registered successfully",
                    usercode = user.Usercode,
                    email = user.UserEmail
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new { message = "Internal server error" });
            }
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
            try
            {
                if (string.IsNullOrWhiteSpace(userDTO.UserEmail) || string.IsNullOrWhiteSpace(userDTO.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }

                var userCode = await _userService.GetUserCodeAsync(userDTO.UserEmail, userDTO.Password);
                
                if (userCode != null)
                {
                    return Ok(new { 
                        message = "Login successful",
                        apikey = userCode,
                        email = userDTO.UserEmail
                    });
                }
                
                _logger.LogWarning($"Failed login attempt for email: {userDTO.UserEmail}");
                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "Internal server error" });
            }
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
            try
            {
                var apiKey = HttpContext.Items["ApiKey"]?.ToString();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Unauthorized(new { message = "API key is required" });
                }

                var deleted = await _userService.DeleteUserAsync(apiKey);
                
                if (deleted)
                {
                    return Ok(new { message = "User account deleted successfully" });
                }
                
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user account");
                return StatusCode(500, new { message = "Internal server error" });
            }
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
            try
            {
                if (string.IsNullOrEmpty(passwordUpdate?.NewPassword))
                {
                    return BadRequest(new { message = "New password is required" });
                }

                // Validate password
                var (isValid, errors) = ValidationHelper.ValidatePassword(passwordUpdate.NewPassword);
                if (!isValid)
                {
                    return BadRequest(new { message = "Password validation failed", errors });
                }

                var apiKey = HttpContext.Items["ApiKey"]?.ToString();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Unauthorized(new { message = "API key is required" });
                }

                var updatedUser = await _userService.UpdateUserPasswordAsync(apiKey, passwordUpdate.NewPassword);
                
                if (updatedUser != null)
                {
                    return Ok(new { message = "Password updated successfully" });
                }
                
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user password");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}