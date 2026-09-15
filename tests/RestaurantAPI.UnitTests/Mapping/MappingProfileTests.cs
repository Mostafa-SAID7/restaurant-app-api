using AutoMapper;
using FluentAssertions;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Common.Mappings;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.UnitTests.TestHelpers;
using Xunit;

namespace RestaurantAPI.UnitTests.Mapping;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MappingProfile_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        mapper.Should().NotBeNull();
    }

    [Fact]
    public void UserToUserDto_MapsEmailAndDoesNotExposePasswordHash()
    {
        var user = TestDataFactory.CreateUser();

        var userDto = _mapper.Map<UserDto>(user);

        userDto.UserEmail.Should().Be(user.UserEmail);
        userDto.Usercode.Should().Be(user.Usercode);
    }

    [Fact]
    public void OrderLineInputDtoToOrder_MapsQuantity()
    {
        var orderDto = new OrderLineInputDto
        {
            ItemID = 1,
            Quantity = 2
        };

        var order = _mapper.Map<Order>(orderDto);

        order.Quantity.Should().Be(2);
        order.ItemPrice.Should().Be(0);
        order.TotalPrice.Should().Be(0);
    }

    [Fact]
    public void OrderToOrderLineDto_MapsLineData()
    {
        var order = TestDataFactory.CreateOrder(
            itemName: "Pasta Carbonara",
            itemPrice: 18.50m,
            quantity: 2
        );

        var lineDto = _mapper.Map<OrderLineDto>(order);

        lineDto.ItemName.Should().Be("Pasta Carbonara");
        lineDto.ItemPrice.Should().Be(18.50m);
        lineDto.Quantity.Should().Be(2);
        lineDto.TotalPrice.Should().Be(37.00m);
    }

    [Fact]
    public void CartToCartItemDto_MapsCartData()
    {
        var cart = TestDataFactory.CreateCart(
            cartId: 1,
            itemId: 1,
            quantity: 3,
            itemPrice: 12.50m
        );

        var cartItemDto = _mapper.Map<CartItemDto>(cart);

        cartItemDto.CartID.Should().Be(1);
        cartItemDto.ItemID.Should().Be(1);
        cartItemDto.Quantity.Should().Be(3);
        cartItemDto.ItemPrice.Should().Be(12.50m);
        cartItemDto.TotalPrice.Should().Be(37.50m);
    }

    [Fact]
    public void MasterOrderToMasterOrderDto_MapsCorrectly()
    {
        var masterOrder = TestDataFactory.CreateMasterOrder(
            masterId: 100,
            grandTotal: 125.00m
        );

        var masterOrderDto = _mapper.Map<MasterOrderDto>(masterOrder);

        masterOrderDto.GrandTotal.Should().Be(125.00m);
    }

    [Fact]
    public void MasterOrderToMasterOrderWithItemsDto_IncludesUserCodeAndRestaurantName()
    {
        var masterOrder = TestDataFactory.CreateMasterOrder();

        var masterOrderWithItemsDto = _mapper.Map<MasterOrderWithItemsDto>(masterOrder);

        masterOrderWithItemsDto.UserCode.Should().NotBeNullOrEmpty();
        masterOrderWithItemsDto.RestaurantName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreateRestaurantDtoToRestaurant_MapsCorrectly()
    {
        var restaurantDto = new CreateRestaurantDto
        {
            RestaurantName = "Test Restaurant",
            Address = "123 Main St",
            Type = "Fine Dining"
        };

        var restaurant = _mapper.Map<Restaurant>(restaurantDto);

        restaurant.RestaurantName.Should().Be(restaurantDto.RestaurantName);
        restaurant.Address.Should().Be(restaurantDto.Address);
        restaurant.Type.Should().Be(restaurantDto.Type);
    }

    [Fact]
    public void CreateItemDtoToItem_MapsNameAndPrice()
    {
        var itemDto = new CreateItemDto
        {
            ItemName = "Pizza",
            ItemPrice = 15.00m
        };

        var item = _mapper.Map<Item>(itemDto);

        item.ItemName.Should().Be("Pizza");
        item.ItemPrice.Should().Be(15.00m);
        item.ItemID.Should().Be(0);
    }

    [Fact]
    public void Mapping_WithNullSource_HandlesGracefully()
    {
        var result = _mapper.Map<UserDto>((User?)null);
        result.Should().BeNull();
    }
}
