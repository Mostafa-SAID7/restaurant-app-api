using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Auth.Services.Implementation;
using RestaurantAPI.Auth.Services.Interfaces;

namespace RestaurantAPI.Auth.Extensions;

/// <summary>
/// Extension methods for registering auth services into the dependency injection container.
/// Centralizes JWT configuration, token validation, and service registration.
/// Called from Program.cs during app startup.
/// </summary>
public static class AuthServiceCollectionExtensions
{
    /// <summary>
    /// Registers authentication services: IPasswordService, ITokenService, IAuthService, ICurrentUserService.
    /// Called before AddAuthorization().
    /// </summary>
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register core auth services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register HttpContextAccessor for ICurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Configures JWT Bearer authentication scheme.
    /// Sets up token validation: signature, issuer, audience, expiration.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "RestaurantAPI";
        var audience = jwtSettings["Audience"] ?? "RestaurantAPIClients";

        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // No tolerance for expiration
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };

            // Custom error response for failed authentication
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        success = false,
                        message = "Unauthorized - valid JWT token required",
                        timestamp = DateTime.UtcNow
                    };

                    return context.Response.WriteAsJsonAsync(response);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        success = false,
                        message = "Forbidden - insufficient permissions",
                        timestamp = DateTime.UtcNow
                    };

                    return context.Response.WriteAsJsonAsync(response);
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Registers authorization policies for role-based and policy-based access control.
    /// Called after AddAuthentication().
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Policy: User must be authenticated
            options.AddPolicy(RestaurantAPI.Auth.Policies.AuthorizationPolicies.Authenticated, policy =>
            {
                policy.RequireAuthenticatedUser();
            });

            // Policy: User must have Admin role
            options.AddPolicy(RestaurantAPI.Auth.Policies.AuthorizationPolicies.AdminOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });

            // Policy: User must be Customer or RestaurantOwner
            options.AddPolicy(RestaurantAPI.Auth.Policies.AuthorizationPolicies.CustomerOrOwner, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Customer", "RestaurantOwner");
            });

            // Policy: User must have RestaurantOwner role
            options.AddPolicy(RestaurantAPI.Auth.Policies.AuthorizationPolicies.OwnerOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("RestaurantOwner");
            });

            // Policy: User can modify own resource (requires handler)
            options.AddPolicy(RestaurantAPI.Auth.Policies.AuthorizationPolicies.CanModifyOwnResource, policy =>
            {
                policy.AddRequirements(new RestaurantAPI.Auth.Policies.CanModifyOwnResourceRequirement());
            });

            // Policy: User can manage restaurant (RestaurantOwner or Admin)
            options.AddPolicy(RestaurantAPI.Auth.Policies.AuthorizationPolicies.CanManageRestaurant, policy =>
            {
                policy.AddRequirements(new RestaurantAPI.Auth.Policies.CanManageRestaurantRequirement());
            });
        });

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, RestaurantAPI.Auth.Policies.CanModifyOwnResourceHandler>();
        services.AddScoped<IAuthorizationHandler, RestaurantAPI.Auth.Policies.CanManageRestaurantHandler>();

        return services;
    }

    /// <summary>
    /// Middleware configuration for authentication and authorization.
    /// Called from Program.cs with app.UseAuth().
    /// Must be called AFTER UseRouting() and BEFORE MapControllers().
    /// </summary>
    public static WebApplication UseAuthMiddleware(this WebApplication app)
    {
        // Add authentication middleware (validates JWT tokens)
        app.UseAuthentication();

        // Add authorization middleware (enforces policies/roles)
        app.UseAuthorization();

        return app;
    }
}
