using AutoMapper;
using FakeRestuarantAPI.Data;
using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FakeRestuarantAPI.Services.Implementation;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<User> RegisterUserAsync(UserDTO userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        user.Usercode = await GenerateUniqueUserCodeAsync();

        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UserExistsAsync(string userEmail)
    {
        return await _context.User.AnyAsync(u => u.UserEmail == userEmail);
    }

    public async Task<string?> GetUserCodeAsync(string userEmail, string password)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.UserEmail == userEmail);
        
        if (user != null && password == user.Password)
        {
            return user.Usercode;
        }
        
        return null;
    }

    public async Task<User?> GetUserByCodeAsync(string userCode)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.Usercode == userCode);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.User.ToListAsync();
    }

    public async Task<bool> DeleteUserAsync(string apiKey)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        
        if (user != null)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }

    public async Task<User?> UpdateUserPasswordAsync(string apiKey, string newPassword)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        
        if (user != null)
        {
            user.Password = newPassword;
            await _context.SaveChangesAsync();
            return user;
        }
        
        return null;
    }

    public async Task<string> GenerateUniqueUserCodeAsync()
    {
        string userCode;
        bool isUnique;

        do
        {
            userCode = Guid.NewGuid().ToString();
            isUnique = !await _context.User.AnyAsync(r => r.Usercode == userCode);
        } 
        while (!isUnique);

        return userCode;
    }
}