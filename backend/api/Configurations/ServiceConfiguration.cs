using FakeRestuarantAPI.Mapping;
using FakeRestuarantAPI.Services.Interfaces;
using FakeRestuarantAPI.Services.Implementation;

namespace FakeRestuarantAPI.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Register Application Services
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IImageService, ImageService>();

        return services;
    }
}