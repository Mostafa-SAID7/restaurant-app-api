using AutoMapper;
using RestaurantAPI.Auth.Services.Interfaces;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

/// <summary>
/// User service for profile management and user operations
/// Authentication and password changes are delegated to IAuthService
/// Phase A.5: Returns DTOs only, never entities
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Registers a new user with hashed password (delegated to IPasswordService)
    /// </summary>
    public async Task<UserDTO> RegisterUserAsync(UserDTO userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        user.Usercode = Guid.NewGuid().ToString(); // Keep for backward compat, not used for auth
        user.PasswordHash = _passwordService.HashPassword(userDTO.Password);
        user.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDTO>(user);
    }

    /// <summary>
    /// Checks if a user exists by email
    /// </summary>
    public async Task<bool> UserExistsAsync(string userEmail)
    {
        return await _unitOfWork.Users.EmailExistsAsync(userEmail);
    }

    /// <summary>
    /// Gets all registered users (warning: sensitive operation, should be restricted)
    /// </summary>
    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDTO>>(users);
    }

    /// <summary>
    /// Gets user by ID (for profile retrieval)
    /// </summary>
    public async Task<UserDTO?> GetUserByIdAsync(string userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }

    /// <summary>
    /// Deletes a user account by ID
    /// </summary>
    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return false;

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}