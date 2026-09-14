using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using RestaurantAPI.DTOs;
using Xunit;

namespace RestaurantAPI.IntegrationTests.Orders;

public class OrderFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public OrderFlowTests(CustomWebApplicationFactory factory)
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

    private async Task SeedRestaurantAndItems()
    {
        var client = _factory.CreateClient();
        
        // Create restaurant
        var restaurantDto = new RestaurantDTO
        {
            RestaurantName = "Test Restaurant",
            Address = "123 Main St",
            Type = "Fine Dining",
            ParkingLot = true
        };
        
        var restaurantContent = new StringContent(
            JsonSerializer.Serialize(restaurantDto),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync("/api/restaurant", restaurantContent);
    }

    [Fact]
    public async Task CreateOrder_HappyPath_Success()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("order@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO>
            {
                new OrderDTO { ItemID = 1, Quantity = 2 }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(menuDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/api/order/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateOrder_WithoutApiKey_Unauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO> { new OrderDTO { ItemID = 1, Quantity = 1 } }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(menuDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/api/order/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_WithEmptyMenu_BadRequest()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("emptyorder@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO>() // Empty
        };

        var content = new StringContent(
            JsonSerializer.Serialize(menuDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/api/order/create", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_MultipleItems_CalculatesCorrectGrandTotal()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("multiorder@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO>
            {
                new OrderDTO { ItemID = 1, Quantity = 2 },
                new OrderDTO { ItemID = 2, Quantity = 1 }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(menuDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/api/order/create", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseContent.Should().Contain("MasterID");
    }

    [Fact]
    public async Task GetOrdersByUser_ReturnsUserOrders()
    {
        // Arrange
        var userCode = await RegisterAndGetUserCode("getorders@test.com", "ValidPassword123");
        var client = _factory.CreateClientWithApiKey(userCode);

        // Create an order first
        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO>
            {
                new OrderDTO { ItemID = 1, Quantity = 1 }
            }
        };

        var createContent = new StringContent(
            JsonSerializer.Serialize(menuDto),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync("/api/order/create", createContent);

        // Act
        var getResponse = await client.GetAsync($"/api/order/user/{userCode}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
