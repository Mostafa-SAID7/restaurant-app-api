using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Data;

namespace RestaurantAPI.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// Replaces SQL Server with InMemory database for fast, isolated testing
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set test environment
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove ALL DbContext-related services to prevent provider conflicts
            var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove SQL Server provider-related services
            var sqlDescriptors = services
                .Where(sd => sd.ServiceType.Name.Contains("SqlServer") || 
                             (sd.ImplementationType != null && sd.ImplementationType.Name.Contains("SqlServer")))
                .ToList();
            
            foreach (var descriptor in sqlDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for testing with a unique name per factory instance
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        });

        // Call base after service reconfiguration
        base.ConfigureWebHost(builder);
    }
}

/// <summary>
/// Extension methods for CustomWebApplicationFactory
/// </summary>
public static class WebApplicationFactoryExtensions
{
    /// <summary>
    /// Gets an HttpClient with the test server
    /// </summary>
    public static HttpClient CreateClientWithApiKey(
        this WebApplicationFactory<Program> factory,
        string apiKey)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        return client;
    }

    /// <summary>
    /// Gets a scoped DbContext for test setup/verification
    /// </summary>
    public static AppDbContext GetDbContext(this WebApplicationFactory<Program> factory)
    {
        var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
