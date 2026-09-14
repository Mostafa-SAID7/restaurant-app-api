using AutoMapper;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using RestuarantAPI.Services.Interfaces;

namespace RestuarantAPI.Services.Implementation;

/// <summary>
/// User service handling user registration, authentication, and account management
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Registers a new user with hashed password
    /// </summary>
    public async Task<User> RegisterUserAsync(UserDTO userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        user.Usercode = await GenerateUniqueUserCodeAsync();
        
        // Hash password before storing
        user.PasswordHash = _unitOfWork.Users.HashPassword(user, userDTO.Password);
        user.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Checks if a user exists by email
    /// </summary>
    public async Task<bool> UserExistsAsync(string userEmail)
    {
        return await _unitOfWork.Users.EmailExistsAsync(userEmail);
    }

    /// <summary>
    /// Authenticates user and returns API key (Usercode) if credentials are valid
    /// </summary>
    public async Task<string?> GetUserCodeAsync(string userEmail, string password)
    {
        var user = await _unitOfWork.Users.ValidateUserAsync(userEmail, password);
        return user?.Usercode;
    }

    /// <summary>
    /// Retrieves user by their API key (Usercode)
    /// </summary>
    public async Task<User?> GetUserByCodeAsync(string userCode)
    {
        return await _unitOfWork.Users.GetByUserCodeAsync(userCode);
    }

    /// <summary>
    /// Gets all registered users (warning: sensitive operation, should be restricted)
    /// </summary>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }

    /// <summary>
    /// Deletes a user account
    /// </summary>
    public async Task<bool> DeleteUserAsync(string apiKey)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null) return false;

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates user password with hashing
    /// </summary>
    public async Task<User?> UpdateUserPasswordAsync(string apiKey, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null) return null;

        // Hash new password before storing
        user.PasswordHash = _unitOfWork.Users.HashPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Generates a unique user code (API key) for authentication
    /// </summary>
    public async Task<string> GenerateUniqueUserCodeAsync()
    {
        string userCode;
        do
        {
            userCode = Guid.NewGuid().ToString();
        } while (await _unitOfWork.Users.UserCodeExistsAsync(userCode));

        return userCode;
    }
}