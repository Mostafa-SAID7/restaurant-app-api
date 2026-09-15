using RestaurantAPI.Mapping;
using RestaurantAPI.DTOs.External;
using RestaurantAPI.Services.Interfaces;
using RestaurantAPI.Services.Implementation;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Repositories.Implementation;
using RestaurantAPI.Auth.Extensions;
using RestaurantAPI.Application.Features.Orders.Authorization;

namespace RestaurantAPI.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add AutoMapper with both internal and external profiles
        services.AddAutoMapper(typeof(MappingProfile), typeof(ExternalMappingProfile));

        // Register Auth Services (must be before authorization)
        services.AddAuthServices(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationPolicies();

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IMasterOrderRepository, MasterOrderRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Order Authorization Service (DIP: orders module uses this for authorization)
        services.AddScoped<IOrderAuthorizationService, OrderAuthorizationService>();

        // Register Application Services
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IImageService, ImageService>();

        return services;
    }
}