using RestuarantAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace RestuarantAPI.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // Global filters
            options.Filters.Add<GlobalExceptionFilter>();
            options.Filters.Add<ValidationFilter>();
            
            // Global settings
            options.SuppressAsyncSuffixInActionNames = false;
        })
        .AddJsonOptions(options =>
        {
            // Configure JSON serialization
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep original property names
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // Configure model validation
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                return new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = errors
                });
            };
        });

        return services;
    }
}