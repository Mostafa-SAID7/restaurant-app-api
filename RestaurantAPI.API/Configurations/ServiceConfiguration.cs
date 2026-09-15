using RestaurantAPI.Infrastructure.Persistence;
using RestaurantAPI.API.Extensions;
using RestaurantAPI.Application.Features.Orders.Authorization;
using RestaurantAPI.Infrastructure.Services;
using RestaurantAPI.Application;
using RestaurantAPI.Application.Common.Abstractions;

namespace RestaurantAPI.API.Configurations;

public static class ServiceConfiguration
{
    /// <summary>
    /// Registers all API-layer services: Application services (MediatR/AutoMapper/Validators),
    /// auth services, JWT configuration, authorization policies, and infrastructure utilities.
    /// Renamed from AddApplicationServices to avoid collision with
    /// RestaurantAPI.Application.DependencyInjection.AddApplicationServices().
    /// </summary>
    public static IServiceCollection AddApiLayerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Application layer services (MediatR, FluentValidation, AutoMapper)
        services.AddApplicationServices();

        // Register Auth Services (must be before authorization)
        services.AddAuthServices(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationPolicies();

        // Image Service (infrastructure utility, not business logic)
        services.AddScoped<IImageService, ImageService>();

        // Register Order Authorization Service (DIP: orders module uses this for authorization)
        services.AddScoped<IOrderAuthorizationService, OrderAuthorizationService>();

        return services;
    }
}