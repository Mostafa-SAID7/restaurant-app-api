using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Application.Common.Abstractions;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Infrastructure.Persistence;

namespace RestaurantAPI.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                .ToList();

            foreach (var descriptor in toRemove)
            {
                services.Remove(descriptor);
            }

            var sqlDescriptors = services
                .Where(sd => sd.ServiceType.Name.Contains("SqlServer") ||
                             (sd.ImplementationType != null && sd.ImplementationType.Name.Contains("SqlServer")))
                .ToList();

            foreach (var descriptor in sqlDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        });

        base.ConfigureWebHost(builder);
    }
}

public static class WebApplicationFactoryExtensions
{
    public static HttpClient CreateClientWithBearerToken(
        this WebApplicationFactory<Program> factory,
        string accessToken)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    public static AppDbContext GetDbContext(this WebApplicationFactory<Program> factory)
    {
        var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public static StringContent ToJsonContent(this object payload)
    {
        return new StringContent(
            JsonSerializer.Serialize(payload, CustomWebApplicationFactory.JsonOptions),
            Encoding.UTF8,
            "application/json");
    }

    public static async Task<(HttpClient Client, string AccessToken, string UserId)> RegisterAndAuthenticateAsync(
        this CustomWebApplicationFactory factory,
        string email,
        string password)
    {
        var client = factory.CreateClient();
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        };

        var registerResponse = await client.PostAsync("/api/auth/register", registerRequest.ToJsonContent());
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsync("/api/auth/login", new LoginRequestDto
        {
            Email = email,
            Password = password
        }.ToJsonContent());
        loginResponse.EnsureSuccessStatusCode();

        var body = await loginResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var data = doc.RootElement.GetProperty("Data");
        var accessToken = data.GetProperty("AccessToken").GetString()
            ?? throw new InvalidOperationException("AccessToken missing from login response");
        var userId = data.GetProperty("UserId").GetString()
            ?? throw new InvalidOperationException("UserId missing from login response");

        return (factory.CreateClientWithBearerToken(accessToken), accessToken, userId);
    }
}
