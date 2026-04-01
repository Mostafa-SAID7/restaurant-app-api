using FakeRestuarantAPI.Filters;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace FakeRestuarantAPI.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Restaurant API",
                Version = "v1",
                Description = "A comprehensive Restaurant Management API with user authentication, menu management, ordering system, and image upload capabilities.",
                Contact = new OpenApiContact
                {
                    Name = "Restaurant API Support",
                    Email = "support@restaurantapi.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add API Key authentication
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints. Format: 'your-api-key'",
                In = ParameterLocation.Query,
                Name = "apikey",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
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
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "Restaurant API Documentation";
                c.DefaultModelsExpandDepth(-1);
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();
            });
        }

        return app;
    }
}