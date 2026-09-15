using System.Runtime.CompilerServices;
using RestaurantAPI.Configurations;
using RestaurantAPI.Application;
using RestaurantAPI.Infrastructure;
using Serilog;

[assembly: InternalsVisibleTo("RestaurantAPI.UnitTests")]
[assembly: InternalsVisibleTo("RestaurantAPI.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration first
builder.AddLoggingConfiguration();

// Add layer services (Domain → Application → Infrastructure order)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.ConfigureMiddleware();

app.Run();

public partial class Program { }
