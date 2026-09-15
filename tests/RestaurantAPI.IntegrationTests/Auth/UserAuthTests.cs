using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestaurantAPI.Application.Common.DTOs;
using Xunit;

namespace RestaurantAPI.IntegrationTests.Auth;

public class UserAuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string StrongPassword = "ValidPassword123";

    public UserAuthTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_User_Success()
    {
        var registerRequest = new RegisterRequestDto
        {
            Email = "newuser@test.com",
            Password = StrongPassword,
            ConfirmPassword = StrongPassword
        };

        var response = await _client.PostAsync("/api/auth/register", registerRequest.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_BadRequest()
    {
        var registerRequest = new RegisterRequestDto
        {
            Email = "invalid-email",
            Password = StrongPassword,
            ConfirmPassword = StrongPassword
        };

        var response = await _client.PostAsync("/api/auth/register", registerRequest.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithShortPassword_BadRequest()
    {
        var registerRequest = new RegisterRequestDto
        {
            Email = "user@test.com",
            Password = "Short",
            ConfirmPassword = "Short"
        };

        var response = await _client.PostAsync("/api/auth/register", registerRequest.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        var email = "loginuser@test.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            Password = StrongPassword,
            ConfirmPassword = StrongPassword
        };

        await _client.PostAsync("/api/auth/register", registerRequest.ToJsonContent());

        var loginRequest = new LoginRequestDto { Email = email, Password = StrongPassword };
        var response = await _client.PostAsync("/api/auth/login", loginRequest.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("AccessToken");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Unauthorized()
    {
        var email = "testuser@test.com";
        var registerRequest = new RegisterRequestDto
        {
            Email = email,
            Password = StrongPassword,
            ConfirmPassword = StrongPassword
        };

        await _client.PostAsync("/api/auth/register", registerRequest.ToJsonContent());

        var loginRequest = new LoginRequestDto { Email = email, Password = "WrongPassword12" };
        var response = await _client.PostAsync("/api/auth/login", loginRequest.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Unauthorized()
    {
        var response = await _client.GetAsync("/api/cart");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_Unauthorized()
    {
        var clientWithToken = _factory.CreateClientWithBearerToken("invalid-token");

        var response = await clientWithToken.GetAsync("/api/cart");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Succeeds()
    {
        var (_, _, _) = await _factory.RegisterAndAuthenticateAsync("protected@test.com", StrongPassword);
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("protected2@test.com", StrongPassword);

        var response = await client.GetAsync("/api/cart");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
