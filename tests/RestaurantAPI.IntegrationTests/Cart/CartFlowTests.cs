using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using RestaurantAPI.Application.Common.DTOs;
using Xunit;

namespace RestaurantAPI.IntegrationTests.Cart;

public class CartFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CartFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<string> RegisterAndGetUserCode(string email, string password)
    {
        var client = _factory.CreateClient();
        var registerRequest = new UserDTO { UserEmail = email, Password = password };
        var content = new StringContent(
            JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync("/api/user/register", content);

        var loginRequest = new { Email = email, Password = password };
        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/user/login", loginContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.GetProperty("data").GetProperty("usercode").GetString()!;
    }

    [Fact]
    public async Task AddItemToCart_Success()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("cart@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        var setCart = new SetCart
        {
            ItemID = 1,
            Quantity = 2
        };

        var content = new StringContent(
            JsonSerializer.Serialize(setCart),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync($"/api/cart/{userCode}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("ItemID");
    }

    [Fact]
    public async Task AddItemToCart_WithoutApiKey_Unauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var setCart = new SetCart { ItemID = 1, Quantity = 1 };

        var content = new StringContent(
            JsonSerializer.Serialize(setCart),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync($"/api/cart/invalid-key", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCart_ReturnsCartItems()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("getcart@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        // Add item to cart first
        var setCart = new SetCart { ItemID = 1, Quantity = 1 };
        var addContent = new StringContent(
            JsonSerializer.Serialize(setCart),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync($"/api/cart/{userCode}", addContent);

        // Act
        var response = await client.GetAsync($"/api/cart/{userCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("ItemID");
    }

    [Fact]
    public async Task RemoveItemFromCart_Success()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("removecart@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        // Add item to cart first
        var setCart = new SetCart { ItemID = 1, Quantity = 1 };
        var addContent = new StringContent(
            JsonSerializer.Serialize(setCart),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync($"/api/cart/{userCode}", addContent);

        // Act
        var response = await client.DeleteAsync($"/api/cart/{userCode}/items/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ClearCart_Success()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("clearcart@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        // Add items to cart
        var setCart = new SetCart { ItemID = 1, Quantity = 2 };
        var addContent = new StringContent(
            JsonSerializer.Serialize(setCart),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync($"/api/cart/{userCode}", addContent);

        // Act
        var response = await client.DeleteAsync($"/api/cart/{userCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCartSummary_CalculatesGrandTotal()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("summary@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        // Add items with different quantities
        var setCart = new SetCart { ItemID = 1, Quantity = 3 };
        var addContent = new StringContent(
            JsonSerializer.Serialize(setCart),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync($"/api/cart/{userCode}", addContent);

        // Act
        var response = await client.GetAsync($"/api/cart/{userCode}/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("GrandTotal");
        responseContent.Should().Contain("cartitems");
    }

    [Fact]
    public async Task AddMultipleItemsToCart_Success()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("multi@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        // Add first item
        var setCart1 = new SetCart { ItemID = 1, Quantity = 2 };
        var content1 = new StringContent(
            JsonSerializer.Serialize(setCart1),
            Encoding.UTF8,
            "application/json");

        var response1 = await client.PostAsync($"/api/cart/{userCode}", content1);

        // Add second item
        var setCart2 = new SetCart { ItemID = 2, Quantity = 1 };
        var content2 = new StringContent(
            JsonSerializer.Serialize(setCart2),
            Encoding.UTF8,
            "application/json");

        var response2 = await client.PostAsync($"/api/cart/{userCode}", content2);

        // Act
        var summary = await client.GetAsync($"/api/cart/{userCode}/summary");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        summary.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
