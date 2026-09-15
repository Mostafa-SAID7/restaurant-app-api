using System.Runtime.CompilerServices;
using RestaurantAPI.API.Configurations;
using RestaurantAPI.Configurations;
using RestaurantAPI.Data.Seeds;
using RestaurantAPI.Infrastructure;
using Serilog;

[assembly: InternalsVisibleTo("RestaurantAPI.UnitTests")]
[assembly: InternalsVisibleTo("RestaurantAPI.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration first
builder.AddLoggingConfiguration();

// Add layer services (Infrastructure data access, then Application + auth)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiLayerServices(builder.Configuration);

// Add API infrastructure
builder.Services.AddApiConfiguration();
builder.Services.AddFilterConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();

// Add health checks
var healthChecks = builder.Services.AddHealthChecks();

if (!builder.Environment.IsEnvironment("Test"))
{
    healthChecks.AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sql-server",
        tags: new[] { "database", "sql", "sqlserver" });
}

healthChecks.AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"));

var app = builder.Build();

if (!app.Environment.IsEnvironment("Test"))
{
    try
    {
        await app.SeedDataAsync();
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Database migrate/seed skipped. Start SQL Server (see docker-compose.yml) if API data endpoints fail.");
    }
}

app.ConfigureMiddleware();

await app.RunAsync();

public partial class Program { }
