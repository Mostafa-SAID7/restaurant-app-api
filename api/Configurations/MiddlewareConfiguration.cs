namespace RestaurantAPI.Configurations;

using RestaurantAPI.Middleware;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // 1. Security Headers
        app.UseSecurityHeaders();

        // 2. CORS
        app.UseCorsConfiguration();

        // 3. HTTP Redirection (redirect HTTP to HTTPS in production)
        app.UseHttpsRedirection();

        // 4. Static Files (Home.html, Docs.html, 404.html, css, images)
        app.UseStaticFiles();

        // 5. Swagger UI (ONLY in Development - SECURITY FIX)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerConfiguration();
        }

        // 6. Routing
        app.UseRouting();
        app.UseAuthorization();

        // 7. Health Checks
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

        // 8. Explicit Root Redirect to the New Home Page
        app.MapGet("/", () => Results.Redirect("/Home.html"));

        // 9. Controllers
        app.MapControllers();

        // 10. Custom 404 Fallback for all other unmatched routes
        app.MapFallback(async context =>
        {
            context.Response.Redirect("/404.html");
            await Task.CompletedTask;
        });

        return app;
    }
}