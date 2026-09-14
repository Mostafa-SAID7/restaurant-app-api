namespace RestaurantAPI.Configurations;

using RestaurantAPI.Middleware;
using RestaurantAPI.Auth.Extensions;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Middleware pipeline order (critical for correct request handling):
        // Exception handling is managed by GlobalExceptionFilter (registered in ApiConfiguration)
        // This ensures all exceptions return JSON responses, not HTML error pages

        // 1. HSTS (HTTP Strict Transport Security) - Production only
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        // 2. Security Headers Middleware
        app.UseSecurityHeaders();

        // 3. CORS - Must come before UseRouting for proper CORS handling
        app.UseCorsConfiguration();

        // 4. HTTPS Redirection - Redirect HTTP to HTTPS in production
        app.UseHttpsRedirection();

        // 5. Static Files (Home.html, Docs.html, 404.html, css, images)
        app.UseStaticFiles();

        // 6. Swagger UI - Development only (security best practice)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerConfiguration();
        }

        // 7. Response Wrapper Middleware (Phase B.2) - Track execution time and add request IDs
        app.UseResponseWrapper();

        // 8. Routing
        app.UseRouting();

        // 9. Authentication & Authorization (JWT Bearer tokens)
        app.UseAuthMiddleware();
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        description = x.Value.Description,
                        duration = x.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds,
                    timestamp = DateTime.UtcNow
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });

        // 10. Root Route - Explicit redirect to home page
        app.MapGet("/", () => Results.Redirect("/Home.html"));

        // 11. API Controllers
        app.MapControllers();

        // 12. Fallback 404 Handler - Catch all unmatched routes last
        app.MapFallback(async context =>
        {
            context.Response.Redirect("/404.html");
            await Task.CompletedTask;
        });

        return app;
    }
}