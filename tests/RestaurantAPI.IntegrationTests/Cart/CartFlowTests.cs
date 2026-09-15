using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Infrastructure.Persistence;
using Xunit;

namespace RestaurantAPI.IntegrationTests.Cart;

public class CartFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private const string StrongPassword = "ValidPassword123";

    public CartFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private int SeedMenuItem()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var restaurant = new Restaurant
        {
            RestaurantName = "Cart Test Restaurant",
            Address = "1 Test St",
            Type = "Cafe",
            ParkingLot = true
        };
        db.Restaurants.Add(restaurant);
        db.SaveChanges();

        var item = new Item
        {
            ItemName = "Test Item",
            ItemPrice = 10.00m,
            RestaurantID = restaurant.RestaurantID
        };
        db.Items.Add(item);
        db.SaveChanges();
        return item.ItemID;
    }

    [Fact]
    public async Task AddItemToCart_Success()
    {
        var itemId = SeedMenuItem();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("cart@test.com", StrongPassword);

        var response = await client.PostAsync("/api/cart", new AddCartItemDto
        {
            ItemID = itemId,
            Quantity = 2
        }.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("ItemID");
    }

    [Fact]
    public async Task AddItemToCart_WithoutToken_Unauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/cart", new AddCartItemDto
        {
            ItemID = 1,
            Quantity = 1
        }.ToJsonContent());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCart_ReturnsCartItems()
    {
        var itemId = SeedMenuItem();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("getcart@test.com", StrongPassword);

        await client.PostAsync("/api/cart", new AddCartItemDto { ItemID = itemId, Quantity = 1 }.ToJsonContent());

        var response = await client.GetAsync("/api/cart");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("ItemID");
    }

    [Fact]
    public async Task RemoveItemFromCart_Success()
    {
        var itemId = SeedMenuItem();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("removecart@test.com", StrongPassword);

        await client.PostAsync("/api/cart", new AddCartItemDto { ItemID = itemId, Quantity = 1 }.ToJsonContent());

        var response = await client.DeleteAsync($"/api/cart/{itemId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ClearCart_Success()
    {
        var itemId = SeedMenuItem();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("clearcart@test.com", StrongPassword);

        await client.PostAsync("/api/cart", new AddCartItemDto { ItemID = itemId, Quantity = 2 }.ToJsonContent());

        var response = await client.DeleteAsync("/api/cart");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCartSummary_CalculatesGrandTotal()
    {
        var itemId = SeedMenuItem();
        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("summary@test.com", StrongPassword);

        await client.PostAsync("/api/cart", new AddCartItemDto { ItemID = itemId, Quantity = 3 }.ToJsonContent());

        var response = await client.GetAsync("/api/cart/summary");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("GrandTotal");
    }

    [Fact]
    public async Task AddMultipleItemsToCart_Success()
    {
        var itemId = SeedMenuItem();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var second = new Item
        {
            ItemName = "Second Item",
            ItemPrice = 5.00m,
            RestaurantID = db.Restaurants.Select(r => r.RestaurantID).First()
        };
        db.Items.Add(second);
        db.SaveChanges();

        var (client, _, _) = await _factory.RegisterAndAuthenticateAsync("multi@test.com", StrongPassword);

        var response1 = await client.PostAsync("/api/cart", new AddCartItemDto { ItemID = itemId, Quantity = 2 }.ToJsonContent());
        var response2 = await client.PostAsync("/api/cart", new AddCartItemDto { ItemID = second.ItemID, Quantity = 1 }.ToJsonContent());
        var summary = await client.GetAsync("/api/cart/summary");

        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        summary.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
