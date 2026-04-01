namespace RestuarantAPI.Configurations;

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

        // 1. CORS First
        app.UseCorsConfiguration();

        // 2. HTTP Redirection
        app.UseHttpsRedirection();

        // 3. Static Files (Home.html, Docs.html, 404.html, css, images)
        app.UseStaticFiles();

        // 4. Swagger UI (Mounts at root namespace /index.html)
        app.UseSwaggerConfiguration();

        // 5. Routing
        app.UseRouting();
        app.UseAuthorization();

        // 6. Explicit Root Redirect to the New Home Page
        app.MapGet("/", () => Results.Redirect("/Home.html"));

        // 7. Controllers
        app.MapControllers();

        // 8. Custom 404 Fallback for all other unmatched routes
        app.MapFallback(async context =>
        {
            context.Response.Redirect("/404.html");
            await Task.CompletedTask;
        });

        return app;
    }
}