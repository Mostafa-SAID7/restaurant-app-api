using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using RestaurantAPI.Application.Common.DTOs;
using Xunit;

namespace RestaurantAPI.IntegrationTests.Auth;

public class UserAuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserAuthTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_User_Success()
    {
        // Arrange
        var registerRequest = new UserDTO
        {
            UserEmail = "newuser@test.com",
            Password = "ValidPassword123"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/user/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_BadRequest()
    {
        // Arrange
        var registerRequest = new UserDTO
        {
            UserEmail = "invalid-email",
            Password = "ValidPassword123"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/user/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithShortPassword_BadRequest()
    {
        // Arrange
        var registerRequest = new UserDTO
        {
            UserEmail = "user@test.com",
            Password = "Short"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/user/register", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsUserCode()
    {
        // Arrange - First register a user
        var email = "loginuser@test.com";
        var password = "ValidPassword123";
        
        var registerRequest = new UserDTO { UserEmail = email, Password = password };
        var registerContent = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        await _client.PostAsync("/api/user/register", registerContent);

        // Now login
        var loginRequest = new { Email = email, Password = password };
        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/user/login", loginContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("usercode");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Unauthorized()
    {
        // Arrange - First register a user
        var email = "testuser@test.com";
        var password = "CorrectPassword123";
        
        var registerRequest = new UserDTO { UserEmail = email, Password = password };
        var registerContent = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        await _client.PostAsync("/api/user/register", registerContent);

        // Try to login with wrong password
        var loginRequest = new { Email = email, Password = "WrongPassword" };
        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/user/login", loginContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutApiKey_Unauthorized()
    {
        // Act - Try to access protected endpoint without API key
        var response = await _client.GetAsync("/api/cart/test-key");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidApiKey_Unauthorized()
    {
        // Arrange
        var invalidKey = Guid.NewGuid().ToString();
        var clientWithKey = _factory.CreateClientWithApiKey(invalidKey);

        // Act
        var response = await clientWithKey.GetAsync("/api/cart/" + invalidKey);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidApiKey_Succeeds()
    {
        // Arrange - Register and login to get API key
        var email = "protected@test.com";
        var password = "ValidPassword123";
        
        var registerRequest = new UserDTO { UserEmail = email, Password = password };
        var registerContent = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        await _client.PostAsync("/api/user/register", registerContent);

        var loginRequest = new { Email = email, Password = password };
        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _client.PostAsync("/api/user/login", loginContent);
        var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
        
        // Extract usercode from response
        using var doc = JsonDocument.Parse(loginResponseContent);
        var usercode = doc.RootElement.GetProperty("data").GetProperty("usercode").GetString();

        var clientWithKey = _factory.CreateClientWithApiKey(usercode!);

        // Act - Access protected endpoint with valid key
        var response = await clientWithKey.GetAsync($"/api/cart/{usercode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
