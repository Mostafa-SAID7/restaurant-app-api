using AutoMapper;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using RestuarantAPI.Services.Interfaces;

namespace RestuarantAPI.Services.Implementation;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<User> RegisterUserAsync(UserDTO userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        user.Usercode = await GenerateUniqueUserCodeAsync();

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<bool> UserExistsAsync(string userEmail)
    {
        return await _unitOfWork.Users.EmailExistsAsync(userEmail);
    }

    public async Task<string?> GetUserCodeAsync(string userEmail, string password)
    {
        var user = await _unitOfWork.Users.ValidateUserAsync(userEmail, password);
        return user?.Usercode;
    }

    public async Task<User?> GetUserByCodeAsync(string userCode)
    {
        return await _unitOfWork.Users.GetByUserCodeAsync(userCode);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }

    public async Task<bool> DeleteUserAsync(string apiKey)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null) return false;

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<User?> UpdateUserPasswordAsync(string apiKey, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null) return null;

        user.Password = newPassword;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

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