using System.Security.Claims;
using RestaurantAPI.Auth.Services.Interfaces;

namespace RestaurantAPI.Auth.Services.Implementation;

/// <summary>
/// Implementation of ICurrentUserService using ClaimsPrincipal from HttpContext.
/// Extracts authenticated user information from JWT claims.
/// Replaces manual HttpContext.Items lookups with clean abstraction.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string? UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }
    }

    public string? UserEmail
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email);
            return claim?.Value;
        }
    }

    public IEnumerable<string> Roles
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role) ?? [];
            return claims.Select(c => c.Value).ToList();
        }
    }

    public bool HasRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public bool HasAnyRole(params string[] roles)
    {
        var userRoles = Roles.ToList();
        return roles.Any(role => userRoles.Contains(role));
    }

    public bool IsAuthenticated
    {
        get
        {
            var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            return isAuthenticated;
        }
    }

    public string? GetClaimValue(string claimType)
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
        return claim?.Value;
    }

    public IEnumerable<Claim> GetClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? [];
    }
}
