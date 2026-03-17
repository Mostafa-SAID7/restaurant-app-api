using FakeRestuarantAPI.Models;

namespace FakeRestuarantAPI.Services.Interfaces;

public interface IUserService
{
    Task<User> RegisterUserAsync(UserDTO userDTO);
    Task<bool> UserExistsAsync(string userEmail);
    Task<string?> GetUserCodeAsync(string userEmail, string password);
    Task<User?> GetUserByCodeAsync(string userCode);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(string apiKey);
    Task<User?> UpdateUserPasswordAsync(string apiKey, string newPassword);
    Task<string> GenerateUniqueUserCodeAsync();
}