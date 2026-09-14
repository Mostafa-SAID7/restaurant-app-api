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

        // 7. Explicit Root Redirect to the New Home Page
        app.MapGet("/", () => Results.Redirect("/Home.html"));

        // 8. Controllers
        app.MapControllers();

        // 9. Custom 404 Fallback for all other unmatched routes
        app.MapFallback(async context =>
        {
            context.Response.Redirect("/404.html");
            await Task.CompletedTask;
        });

        return app;
    }
}