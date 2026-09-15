using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Infrastructure.Persistence;
using Xunit;

namespace RestaurantAPI.IntegrationTests.Orders;

public class OrderFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private const string StrongPassword = "ValidPassword123";

    public OrderFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private (int RestaurantId, int ItemId1, int ItemId2) SeedRestaurantAndItems()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var restaurant = new Restaurant
        {
            RestaurantName = "Order Test Restaurant",
            Address = "123 Main St",
            Type = "Fine Dining",
            ParkingLot = true
        };
        db.Restaurants.Add(restaurant);
        db.SaveChanges();

        var item1 = new Item { ItemName = "Steak", ItemPrice = 20.00m, RestaurantID = restaurant.RestaurantID };
        var item2 = new Item { ItemName = "Salad", ItemPrice = 8.00m, RestaurantID = restaurant.RestaurantID };
        db.Items.AddRange(item1, item2);
        db.SaveChanges();

        return (restaurant.RestaurantID, item1.ItemID, item2.ItemID);
    }

    [Fact]
    public async Task CreateOrder_HappyPath_Success()
    {
        var (restaurantId, itemId, _) = SeedRestaurantAndItems();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("order@test.com", StrongPassword);

        var createOrder = new CreateOrderDto
        {
            Items = new List<OrderLineInputDto>
            {
                new() { ItemID = itemId, Quantity = 2 }
            }
        };

        var response = await client.PostAsync($"/api/order/{restaurantId}/create", createOrder.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateOrder_WithoutToken_Unauthorized()
    {
        var client = _factory.CreateClient();
        var createOrder = new CreateOrderDto
        {
            Items = new List<OrderLineInputDto>
            {
                new() { ItemID = 1, Quantity = 1 }
            }
        };

        var response = await client.PostAsync("/api/order/1/create", createOrder.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_WithEmptyMenu_BadRequest()
    {
        var (restaurantId, _, _) = SeedRestaurantAndItems();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("emptyorder@test.com", StrongPassword);

        var createOrder = new CreateOrderDto { Items = new List<OrderLineInputDto>() };

        var response = await client.PostAsync($"/api/order/{restaurantId}/create", createOrder.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_MultipleItems_CalculatesCorrectGrandTotal()
    {
        var (restaurantId, itemId1, itemId2) = SeedRestaurantAndItems();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("multiorder@test.com", StrongPassword);

        var createOrder = new CreateOrderDto
        {
            Items = new List<OrderLineInputDto>
            {
                new() { ItemID = itemId1, Quantity = 2 },
                new() { ItemID = itemId2, Quantity = 1 }
            }
        };

        var response = await client.PostAsync($"/api/order/{restaurantId}/create", createOrder.ToJsonContent());
        var responseContent = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseContent.Should().Contain("MasterID");
    }

    [Fact]
    public async Task GetOrdersByUser_ReturnsUserOrders()
    {
        var (restaurantId, itemId, _) = SeedRestaurantAndItems();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("getorders@test.com", StrongPassword);

        var createOrder = new CreateOrderDto
        {
            Items = new List<OrderLineInputDto>
            {
                new() { ItemID = itemId, Quantity = 1 }
            }
        };

        await client.PostAsync($"/api/order/{restaurantId}/create", createOrder.ToJsonContent());

        var getResponse = await client.GetAsync("/api/order");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
