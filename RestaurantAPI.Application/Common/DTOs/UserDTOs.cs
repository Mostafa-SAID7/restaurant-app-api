namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// DTO for user profile response.
/// </summary>
public class UserDto
{
    public string Usercode { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for authenticated user information (roles, lock status).
/// Used internally by auth handlers and GetUserAsync.
/// </summary>
public class AuthUserDto
{
    public string Usercode { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
    public bool IsLocked { get; set; }
}
