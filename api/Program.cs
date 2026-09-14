using System.Runtime.CompilerServices;
using RestaurantAPI.Configurations;

[assembly: InternalsVisibleTo("RestaurantAPI.UnitTests")]
[assembly: InternalsVisibleTo("RestaurantAPI.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiConfiguration();
builder.Services.AddFilterConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

// Add health checks
var healthChecks = builder.Services.AddHealthChecks();

// Only add SQL Server health check if not in test environment
if (!builder.Environment.IsEnvironment("Test"))
{
    healthChecks.AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sql-server",
        tags: new[] { "database", "sql", "sqlserver" });
}

healthChecks.AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"));

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigureMiddleware();

app.Run();

public partial class Program { }
