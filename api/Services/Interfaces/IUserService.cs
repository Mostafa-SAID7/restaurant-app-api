using RestaurantAPI.DTOs;

namespace RestaurantAPI.Services.Interfaces;

/// <summary>
/// User service for profile management operations only
/// Authentication and password changes are handled by IAuthService
/// Phase A.5: Returns DTOs only, never entities
/// </summary>
public interface IUserService
{
    Task<UserDTO> RegisterUserAsync(UserDTO userDTO);
    Task<bool> UserExistsAsync(string userEmail);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(string userId);
    Task<UserDTO?> GetUserByIdAsync(string userId);
}