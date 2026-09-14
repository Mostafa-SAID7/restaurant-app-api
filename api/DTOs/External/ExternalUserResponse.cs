namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for user profile responses
/// Phase B.1: Decouples external API contract from internal UserDTO
/// Excludes password hash and sensitive internal fields
/// </summary>
public class ExternalUserResponse
{
    public string Usercode { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// External DTO for user list responses
/// </summary>
public class ExternalUserListResponse
{
    public IEnumerable<ExternalUserResponse> Users { get; set; }
    public int TotalCount { get; set; }
}
