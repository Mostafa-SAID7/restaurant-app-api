namespace FakeRestuarantAPI.Configurations;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Use CORS
        app.UseCorsConfiguration();

        // Static files FIRST — Docs.html is the default landing page at /
        // This must come before Swagger so that / serves Docs.html, not Swagger
        var defaultFileOptions = new DefaultFilesOptions();
        defaultFileOptions.DefaultFileNames.Clear();
        defaultFileOptions.DefaultFileNames.Add("Docs.html");
        app.UseDefaultFiles(defaultFileOptions);
        app.UseStaticFiles();

        // Swagger at /index.html (after static files so / is not intercepted)
        app.UseSwaggerConfiguration();

        // Security and routing
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        // Map controllers
        app.MapControllers();

        return app;
    }
}