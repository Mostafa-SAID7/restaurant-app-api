using RestuarantAPI.Filters;
using RestuarantAPI.Models;
using RestuarantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace RestuarantAPI.Controllers
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

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register new user", Description = "Create a new user account")]
    [SwaggerResponse(201, "User registered successfully", typeof(User))]
    [SwaggerResponse(401, "User already exists")]
    public async Task<ActionResult> registeruserAsync(UserDTO userDTO)
    {
        try
        {
            var userExists = await _userService.UserExistsAsync(userDTO.UserEmail);
            if (userExists)
            {
                return StatusCode(401, new { message = "user already exists" });
            }

            var user = await _userService.RegisterUserAsync(userDTO);
            return StatusCode(201, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("getusercode")]
    [SwaggerOperation(Summary = "Get user code", Description = "Authenticate user and get API key")]
    [SwaggerResponse(200, "Authentication successful")]
    [SwaggerResponse(404, "Invalid credentials")]
    [RateLimit(maxRequests: 10, timeWindowMinutes: 1)] // Rate limit login attempts
    public async Task<ActionResult> getusercode([FromQuery] string UserEmail, string Password)
    {
        try
        {
            var userCode = await _userService.GetUserCodeAsync(UserEmail, Password);
            
            if (userCode != null)
            {
                return Ok(new { usercode = userCode });
            }
            
            return StatusCode(404, new { message = "Invalid Details" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user code");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users", Description = "Retrieve all registered users")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<User>))]
    [SwaggerResponse(404, "No users found")]
    public async Task<ActionResult> getusers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            
            if (users.Any())
            {
                return Ok(users);
            }
            
            return StatusCode(404, "no users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{apikey}")]
    [SwaggerOperation(Summary = "Delete user", Description = "Delete a user account")]
    [SwaggerResponse(200, "User deleted successfully")]
    [SwaggerResponse(404, "User not found")]
    [RequireApiKey] // Require valid API key
    public async Task<ActionResult> DeleteUser(string apikey)
    {
        try
        {
            var deleted = await _userService.DeleteUserAsync(apikey);
            
            if (deleted)
            {
                return StatusCode(200, new { message = "User Data Deleted" });
            }
            
            return StatusCode(404, new { message = "No User Data Found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{apikey}")]
    [SwaggerOperation(Summary = "Update user password", Description = "Update user's password")]
    [SwaggerResponse(201, "Password updated successfully")]
    [SwaggerResponse(404, "User not found")]
    [RequireApiKey] // Require valid API key
    [LogRequests] // Log this sensitive operation
    public async Task<ActionResult> UpdateUser(string apikey, [FromBody] string NewPassword)
    {
        try
        {
            var updatedUser = await _userService.UpdateUserPasswordAsync(apikey, NewPassword);
            
            if (updatedUser != null)
            {
                return StatusCode(201, updatedUser);
            }
            
            return StatusCode(404, new { message = "No User Data Found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return StatusCode(500, "Internal server error");
        }
    }
    }
}
