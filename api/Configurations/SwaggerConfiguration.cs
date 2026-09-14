using RestaurantAPI.Filters;
using OpenApiModels = Microsoft.OpenApi.Models;
using System.Reflection;

namespace RestaurantAPI.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiModels.OpenApiInfo
            {
                Title = "M.Said's Restaurant API",
                Version = "v1",
                Description = "A comprehensive Restaurant Management API by M.Said. Featuring JWT authentication, user roles, menu management, ordering system, and shopping cart. | [GitHub Repository](https://github.com/Mostafa-SAID7/restaurant-app-api) | [Docs](/Docs.html)",
                Contact = new OpenApiModels.OpenApiContact
                {
                    Name = "M.Said",
                    Email = "m.ssaid356@gmail.com",
                    Url = new Uri("https://m-said-portfolio.netlify.app")
                },
                License = new OpenApiModels.OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add JWT Bearer authentication
            c.AddSecurityDefinition("Bearer", new OpenApiModels.OpenApiSecurityScheme
            {
                Description = "JWT Bearer authentication. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = OpenApiModels.ParameterLocation.Header,
                Type = OpenApiModels.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiModels.OpenApiSecurityRequirement
            {
                {
                    new OpenApiModels.OpenApiSecurityScheme
                    {
                        Reference = new OpenApiModels.OpenApiReference
                        {
                            Type = OpenApiModels.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Add file upload support
            c.OperationFilter<FileUploadOperationFilter>();

            // Enable annotations
            c.EnableAnnotations();

            // Group endpoints by tags
            c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            c.DocInclusionPredicate((name, api) => true);
        });

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
            c.RoutePrefix = string.Empty;
            c.DocumentTitle = "M.Said's Restaurant API";
            c.DefaultModelsExpandDepth(-1);
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.ShowExtensions();
            c.EnableValidator();
        });

        return app;
    }
}