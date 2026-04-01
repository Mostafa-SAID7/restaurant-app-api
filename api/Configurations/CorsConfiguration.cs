namespace RestuarantAPI.Configurations;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            options.AddPolicy("Development", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:4200",    // Angular
                    "http://localhost:3000",    // React
                    "http://localhost:5173",    // Vite
                    "http://localhost:8080"     // Vue
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });

            options.AddPolicy("Production", policy =>
            {
                policy.WithOrigins(
                    "https://yourdomain.com",
                    "https://www.yourdomain.com"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication UseCorsConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("Development");
        }
        else
        {
            app.UseCors("Production");
        }

        return app;
    }
}