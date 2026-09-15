using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Application.Common.Mappings;

namespace RestaurantAPI.Application;

/// <summary>
/// Extension methods for registering Application layer services in the DI container.
/// Called from WebApi layer in Program.cs.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR with this assembly
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Register AutoMapper with MappingProfile from this assembly
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }
}
