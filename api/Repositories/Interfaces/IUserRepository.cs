using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// User repository interface with specific user operations
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserCodeAsync(string userCode);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UserCodeExistsAsync(string userCode);
    Task<User?> ValidateUserAsync(string email, string password);
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
}