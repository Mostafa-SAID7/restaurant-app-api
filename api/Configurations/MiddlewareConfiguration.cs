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

        // Use Swagger
        app.UseSwaggerConfiguration();

        // Use CORS
        app.UseCorsConfiguration();

        // Static files
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Security and routing
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        // Map controllers
        app.MapControllers();

        return app;
    }
}