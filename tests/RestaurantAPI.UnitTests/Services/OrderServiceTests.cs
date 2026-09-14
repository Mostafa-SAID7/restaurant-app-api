using AutoMapper;
using FluentAssertions;
using Moq;
using RestaurantAPI.DTOs;
using RestaurantAPI.Mapping;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Implementation;
using RestaurantAPI.UnitTests.TestHelpers;
using Xunit;

namespace RestaurantAPI.UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockUnitOfWork = MockUnitOfWorkFactory.CreateMockUnitOfWork();
        
        // Setup AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        
        _orderService = new OrderService(_mockUnitOfWork.Object, mapper);
    }

    #region CreateOrderAsync Tests

    [Fact]
    public async Task CreateOrderAsync_WithValidInput_CreatesMasterOrderFirst()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);
        var restaurantId = 1;
        var restaurant = TestDataFactory.CreateRestaurant(restaurantId);
        var item = TestDataFactory.CreateItem(itemId: 1, restaurantId: restaurantId, price: 25.00m);

        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO>
            {
                new OrderDTO { ItemID = 1, Quantity = 2 }
            }
        };

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Restaurants.GetByIdAsync(restaurantId))
            .ReturnsAsync(restaurant);

        _mockUnitOfWork.Setup(u => u.Items.GetByRestaurantIdAsync(restaurantId))
            .ReturnsAsync(new List<Item> { item });

        var capturedMasterOrder = new MasterOrder { MasterID = 100, UserID = apiKey, RestaurantID = restaurantId, GrandTotal = 50.00m };
        _mockUnitOfWork.Setup(u => u.MasterOrders.AddAsync(It.IsAny<MasterOrder>()))
            .Callback<MasterOrder>(mo => { mo.MasterID = 100; })
            .ReturnsAsync(capturedMasterOrder);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _orderService.CreateOrderAsync(restaurantId, apiKey, menuDto);

        // Assert
        result.Should().NotBeNull();
        result.MasterID.Should().BeGreaterThan(0);
        _mockUnitOfWork.Verify(u => u.MasterOrders.AddAsync(It.IsAny<MasterOrder>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_RejectedEmptyOrderMenu_ThrowsException()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);

        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO>() // Empty list
        };

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _orderService.CreateOrderAsync(1, apiKey, menuDto)
        );
    }

    [Fact]
    public async Task CreateOrderAsync_RejectsInvalidApiKey_ThrowsException()
    {
        // Arrange
        var invalidKey = Guid.NewGuid().ToString();

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(invalidKey))
            .ReturnsAsync((User?)null);

        var menuDto = new MenuDTO
        {
            menuDTO = new List<OrderDTO> { new OrderDTO { ItemID = 1, Quantity = 1 } }
        };

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _orderService.CreateOrderAsync(1, invalidKey, menuDto)
        );
    }

    #endregion
}
