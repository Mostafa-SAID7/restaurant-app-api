using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// User service for profile management operations only
/// Authentication and password changes are handled by IAuthService
/// Phase A.1: API-key methods removed (JWT replaces Usercode-based auth)
/// </summary>
public interface IUserService
{
    Task<User> RegisterUserAsync(UserDTO userDTO);
    Task<bool> UserExistsAsync(string userEmail);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(string userId);
    Task<User?> GetUserByIdAsync(string userId);
}