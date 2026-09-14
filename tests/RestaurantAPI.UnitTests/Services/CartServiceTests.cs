using FluentAssertions;
using Moq;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Implementation;
using RestaurantAPI.UnitTests.TestHelpers;
using Xunit;

namespace RestaurantAPI.UnitTests.Services;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _mockUnitOfWork = MockUnitOfWorkFactory.CreateMockUnitOfWork();
        _cartService = new CartService(_mockUnitOfWork.Object, null!); // Mapper not needed for these tests
    }

    [Fact]
    public async Task AddItemToCartAsync_WithValidItemID_CreatesCartEntry()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);
        var item = TestDataFactory.CreateItem(itemId: 1, price: 15.50m);
        var setCart = new SetCart { ItemID = 1, Quantity = 2 };

        Cart? capturedCart = null;

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Items.GetByIdAsync(1))
            .ReturnsAsync(item);

        _mockUnitOfWork.Setup(u => u.Carts.AddAsync(It.IsAny<Cart>()))
            .Callback<Cart>(c => capturedCart = c)
            .ReturnsAsync(capturedCart);

        // Act
        var result = await _cartService.AddItemToCartAsync(apiKey, setCart);

        // Assert
        result.Should().NotBeNull();
        result.ItemID.Should().Be(1);
        result.ItemName.Should().Be(item.ItemName);
        result.ItemPrice.Should().Be(15.50m);
        result.Quantity.Should().Be(2);
        result.TotalPrice.Should().Be(31.00m);
        capturedCart?.UserID.Should().Be(apiKey);
    }

    [Fact]
    public async Task AddItemToCartAsync_WithInvalidApiKey_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var invalidKey = Guid.NewGuid().ToString();
        var setCart = new SetCart { ItemID = 1, Quantity = 1 };

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(invalidKey))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _cartService.AddItemToCartAsync(invalidKey, setCart)
        );
    }

    [Fact]
    public async Task AddItemToCartAsync_WithInvalidItemID_ThrowsKeyNotFoundException()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);
        var setCart = new SetCart { ItemID = 999, Quantity = 1 };

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Items.GetByIdAsync(999))
            .ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _cartService.AddItemToCartAsync(apiKey, setCart)
        );
    }

    [Fact]
    public async Task GetCartItemsAsync_ReturnsUserCartItems_MappedToDTO()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);
        var cartItems = new List<Cart>
        {
            TestDataFactory.CreateCart(cartId: 1, userCode: apiKey, itemId: 1, quantity: 2, itemPrice: 10.00m),
            TestDataFactory.CreateCart(cartId: 2, userCode: apiKey, itemId: 2, quantity: 1, itemPrice: 20.00m)
        };

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Carts.GetByUserIdAsync(apiKey))
            .ReturnsAsync(cartItems);

        // Act
        var result = await _cartService.GetCartItemsAsync(apiKey);

        // Assert
        result.Should().HaveCount(2);
        result.First().TotalPrice.Should().Be(20.00m); // 10 * 2
        result.Last().TotalPrice.Should().Be(20.00m);  // 20 * 1
    }

    [Fact]
    public async Task GetCartItemsAsync_WithInvalidApiKey_ThrowsException()
    {
        // Arrange
        var invalidKey = Guid.NewGuid().ToString();

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(invalidKey))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _cartService.GetCartItemsAsync(invalidKey)
        );
    }

    [Fact]
    public async Task RemoveItemFromCartAsync_WithValidItem_RemovesAndReturnsTrue()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Carts.RemoveByUserAndItemAsync(apiKey, 1))
            .ReturnsAsync(true);

        // Act
        var result = await _cartService.RemoveItemFromCartAsync(apiKey, 1);

        // Assert
        result.Should().BeTrue();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveItemFromCartAsync_WithInvalidItem_ReturnsFalse()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Carts.RemoveByUserAndItemAsync(apiKey, 999))
            .ReturnsAsync(false);

        // Act
        var result = await _cartService.RemoveItemFromCartAsync(apiKey, 999);

        // Assert
        result.Should().BeFalse();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ClearCartAsync_RemovesAllItemsForUser()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Carts.ClearByUserIdAsync(apiKey))
            .ReturnsAsync(1);

        // Act
        await _cartService.ClearCartAsync(apiKey);

        // Assert
        _mockUnitOfWork.Verify(u => u.Carts.ClearByUserIdAsync(apiKey), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCartSummaryAsync_CalculatesGrandTotal()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);
        var cartItems = new List<Cart>
        {
            TestDataFactory.CreateCart(cartId: 1, userCode: apiKey, quantity: 2, itemPrice: 10.00m),
            TestDataFactory.CreateCart(cartId: 2, userCode: apiKey, quantity: 3, itemPrice: 15.00m)
        };

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Carts.GetByUserIdAsync(apiKey))
            .ReturnsAsync(cartItems);

        // Act
        var result = await _cartService.GetCartSummaryAsync(apiKey);

        // Assert
        result.Should().NotBeNull();
        result.cartitems.Should().HaveCount(2);
        result.GrandTotal.Should().Be(65.00m); // (10*2) + (15*3) = 20 + 45 = 65
    }
}
