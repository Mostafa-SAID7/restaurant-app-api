using FakeRestuarantAPI.Mapping;
using FakeRestuarantAPI.Services.Interfaces;
using FakeRestuarantAPI.Services.Implementation;
using FakeRestuarantAPI.Repositories.Interfaces;
using FakeRestuarantAPI.Repositories.Implementation;

namespace FakeRestuarantAPI.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IMasterOrderRepository, MasterOrderRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Application Services
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IImageService, ImageService>();

        return services;
    }
}