using Microsoft.Extensions.FileProviders;

namespace FakeRestuarantAPI.Configurations;

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

        // CORS
        app.UseCorsConfiguration();

        // Serve files from api/views/ at the URL root
        // e.g. api/views/Docs.html → GET /Docs.html
        // e.g. api/views/restaurant.css → GET /restaurant.css
        var viewsPath = Path.Combine(app.Environment.ContentRootPath, "views");
        if (Directory.Exists(viewsPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(viewsPath),
                RequestPath = ""
            });
        }

        // Serve wwwroot (images, etc.)
        app.UseStaticFiles();

        // Root / → redirect to Docs
        app.MapGet("/", () => Results.Redirect("/Docs.html"));

        // Swagger at /index.html (RoutePrefix = "" means Swagger serves at root,
        // its embedded UI is served at /index.html)
        app.UseSwaggerConfiguration();

        // Routing & controllers
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}