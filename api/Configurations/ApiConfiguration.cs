using RestaurantAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace RestaurantAPI.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // Global filters - only GlobalExceptionFilter (handles all unhandled exceptions)
            // ValidationFilter removed - validation is handled by InvalidModelStateResponseFactory below
            options.Filters.Add<GlobalExceptionFilter>();
            
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
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Return consistent JSON response matching GlobalExceptionFilter format
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = "Validation failed",
                    Details = errors,
                    Timestamp = DateTime.UtcNow
                });
            };
        });

        return services;
    }
}