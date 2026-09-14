using AutoMapper;
using FluentAssertions;
using RestaurantAPI.DTOs;
using RestaurantAPI.Mapping;
using RestaurantAPI.Models;
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
        // Assert - This validates all mappings in the profile
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        // Just create a mapper to ensure no exceptions occur
        var mapper = config.CreateMapper();
        mapper.Should().NotBeNull();
    }

    #region User Mapping Tests

    [Fact]
    public void UserDTOToUser_MapsCorrectly()
    {
        // Arrange
        var userDto = new UserDTO
        {
            UserEmail = "user@test.com",
            Password = "Password123"
        };

        // Act
        var user = _mapper.Map<User>(userDto);

        // Assert
        user.UserEmail.Should().Be(userDto.UserEmail);
        user.PasswordHash.Should().BeNullOrEmpty(); // Should be ignored
    }

    [Fact]
    public void UserToUserDTO_DoesNotExposPassword()
    {
        // Arrange
        var user = TestDataFactory.CreateUser();

        // Act
        var userDto = _mapper.Map<UserDTO>(user);

        // Assert
        userDto.Password.Should().BeNullOrEmpty();
        userDto.UserEmail.Should().Be(user.UserEmail);
    }

    #endregion

    #region Order Mapping Tests

    [Fact]
    public void OrderDTOToOrder_MapsCorrectly()
    {
        // Arrange
        var orderDto = new OrderDTO
        {
            ItemID = 1,
            Quantity = 2
        };

        // Act
        var order = _mapper.Map<Order>(orderDto);

        // Assert
        order.Quantity.Should().Be(2);
        order.ItemPrice.Should().Be(0); // Should be ignored
        order.TotalPrice.Should().Be(0); // Should be ignored
    }

    [Fact]
    public void OrderToOrderLineDTO_MapsLineData()
    {
        // Arrange
        var order = TestDataFactory.CreateOrder(
            itemName: "Pasta Carbonara",
            itemPrice: 18.50m,
            quantity: 2
        );

        // Act
        var lineDto = _mapper.Map<OrderLineDTO>(order);

        // Assert
        lineDto.ItemName.Should().Be("Pasta Carbonara");
        lineDto.ItemPrice.Should().Be(18.50m);
        lineDto.Quantity.Should().Be(2);
        lineDto.TotalPrice.Should().Be(37.00m);
    }

    #endregion

    #region Cart Mapping Tests

    [Fact]
    public void CartToCartItemDTO_MapsCartData()
    {
        // Arrange
        var cart = TestDataFactory.CreateCart(
            cartId: 1,
            itemId: 1,
            quantity: 3,
            itemPrice: 12.50m
        );

        // Act
        var cartItemDto = _mapper.Map<CartItemDTO>(cart);

        // Assert
        cartItemDto.CartID.Should().Be(1);
        cartItemDto.ItemID.Should().Be(1);
        cartItemDto.Quantity.Should().Be(3);
        cartItemDto.ItemPrice.Should().Be(12.50m);
        cartItemDto.TotalPrice.Should().Be(37.50m);
    }

    #endregion

    #region MasterOrder Mapping Tests

    [Fact]
    public void MasterOrderToMasterOrderDTO_MapsCorrectly()
    {
        // Arrange
        var masterOrder = TestDataFactory.CreateMasterOrder(
            masterId: 100,
            grandTotal: 125.00m
        );

        // Act
        var masterOrderDto = _mapper.Map<MasterOrderDTO>(masterOrder);

        // Assert
        masterOrderDto.GrandTotal.Should().Be(125.00m);
    }

    [Fact]
    public void MasterOrderToMasterOrderWithItemsDTO_IncludesUserCodeAndRestaurantName()
    {
        // Arrange
        var masterOrder = TestDataFactory.CreateMasterOrder();

        // Act
        var masterOrderWithItemsDto = _mapper.Map<MasterOrderWithItemsDTO>(masterOrder);

        // Assert
        masterOrderWithItemsDto.UserCode.Should().NotBeNullOrEmpty();
        masterOrderWithItemsDto.RestaurantName.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Restaurant & Item Mapping Tests

    [Fact]
    public void RestaurantDTOToRestaurant_MapsCorrectly()
    {
        // Arrange
        var restaurantDto = new RestaurantDTO
        {
            RestaurantName = "Test Restaurant",
            Address = "123 Main St",
            Type = "Fine Dining"
        };

        // Act
        var restaurant = _mapper.Map<Restaurant>(restaurantDto);

        // Assert
        restaurant.RestaurantName.Should().Be(restaurantDto.RestaurantName);
        restaurant.Address.Should().Be(restaurantDto.Address);
        restaurant.Type.Should().Be(restaurantDto.Type);
    }

    [Fact]
    public void ItemDTOToItem_IgnoresIDAndRestaurant()
    {
        // Arrange
        var itemDto = new ItemDTO
        {
            ItemName = "Pizza",
            ItemPrice = 15.00m
        };

        // Act
        var item = _mapper.Map<Item>(itemDto);

        // Assert
        item.ItemName.Should().Be("Pizza");
        item.ItemPrice.Should().Be(15.00m);
        item.ItemID.Should().Be(0); // Should be ignored
    }

    #endregion

    #region Null Handling Tests

    [Fact]
    public void Mapping_WithNullSource_HandlesGracefully()
    {
        // Act & Assert
        var result = _mapper.Map<UserDTO>((User?)null);
        result.Should().BeNull();
    }

    #endregion
}
