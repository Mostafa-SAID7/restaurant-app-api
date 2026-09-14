using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// User repository interface for data access only
/// Password hashing moved to IPasswordService (Phase A.2)
/// Usercode methods removed (Phase A.1 - JWT replaces API-key auth)
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> ValidateUserAsync(string email, string password);
}