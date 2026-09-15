using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace RestaurantAPI.Configurations;

/// <summary>
/// Serilog structured logging configuration
/// Phase B.3: Provides structured logging with file and console output
/// </summary>
public static class LoggingConfiguration
{
    public static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName;
        var logDirectory = Path.Combine("logs", environment);
        
        // Ensure log directory exists
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            // Minimum log levels by category
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // Console output (pretty format for development)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")

            // File output (JSON format for production)
            .WriteTo.File(
                path: Path.Combine(logDirectory, "api-.log"),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)

            // Structured JSON logging for cloud/ELK stack
            .WriteTo.File(
                formatter: new Serilog.Formatting.Compact.CompactJsonFormatter(),
                path: Path.Combine(logDirectory, "api-structured-.json"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)

            // Enrich logs with contextual information
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "RestaurantAPI")
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithMachineName()

            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}
