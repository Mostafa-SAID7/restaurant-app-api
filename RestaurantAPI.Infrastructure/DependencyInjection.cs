using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Domain.Interfaces;
using RestaurantAPI.Infrastructure.Persistence;

namespace RestaurantAPI.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services in the DI container.
/// Called from WebApi layer in Program.cs.
/// This is where all EF Core and data access configuration happens.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext with SQL Server
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.GetName().Name);
            });
        });

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
