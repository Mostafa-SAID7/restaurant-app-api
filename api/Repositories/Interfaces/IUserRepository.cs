using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// User repository interface for data access only
/// Phase A.2: Password verification removed (moved to IPasswordService)
/// Phase A.1: Usercode methods removed (JWT replaces API-key auth)
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}