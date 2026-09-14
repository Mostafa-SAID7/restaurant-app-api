using RestaurantAPI.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiConfiguration();
builder.Services.AddFilterConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();

// Add health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sql-server",
        tags: new[] { "database", "sql", "sqlserver" })
    .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"));

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigureMiddleware();

app.Run();
