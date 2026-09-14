using Moq;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;

namespace RestaurantAPI.UnitTests.TestHelpers;

/// <summary>
/// Factory for creating mocked IUnitOfWork instances for unit testing
/// </summary>
public static class MockUnitOfWorkFactory
{
    public static Mock<IUnitOfWork> CreateMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        // Setup repository mocks
        mockUnitOfWork.Setup(u => u.Users).Returns(CreateMockUserRepository().Object);
        mockUnitOfWork.Setup(u => u.Restaurants).Returns(CreateMockRestaurantRepository().Object);
        mockUnitOfWork.Setup(u => u.Items).Returns(CreateMockItemRepository().Object);
        mockUnitOfWork.Setup(u => u.Carts).Returns(CreateMockCartRepository().Object);
        mockUnitOfWork.Setup(u => u.Orders).Returns(CreateMockOrderRepository().Object);
        mockUnitOfWork.Setup(u => u.MasterOrders).Returns(CreateMockMasterOrderRepository().Object);

        // Setup SaveChangesAsync to return 1 (1 row affected)
        mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        return mockUnitOfWork;
    }

    public static Mock<IUserRepository> CreateMockUserRepository()
    {
        var mockRepo = new Mock<IUserRepository>();

        // Default behavior: return null for GetByUserCodeAsync
        mockRepo.Setup(r => r.GetByUserCodeAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        mockRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        mockRepo.Setup(r => r.UserCodeExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        mockRepo.Setup(r => r.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((User?)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User?)null);
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User?)null);
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<User>())).ReturnsAsync(false);
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        return mockRepo;
    }

    public static Mock<IRestaurantRepository> CreateMockRestaurantRepository()
    {
        var mockRepo = new Mock<IRestaurantRepository>();

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Restaurant?)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Restaurant>())).ReturnsAsync((Restaurant?)null);
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Restaurant>());

        return mockRepo;
    }

    public static Mock<IItemRepository> CreateMockItemRepository()
    {
        var mockRepo = new Mock<IItemRepository>();

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Item?)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Item>())).ReturnsAsync((Item?)null);

        return mockRepo;
    }

    public static Mock<ICartRepository> CreateMockCartRepository()
    {
        var mockRepo = new Mock<ICartRepository>();

        mockRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(new List<Cart>());
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Cart>())).ReturnsAsync((Cart?)null);
        mockRepo.Setup(r => r.RemoveByUserAndItemAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);
        mockRepo.Setup(r => r.ClearByUserIdAsync(It.IsAny<string>())).ReturnsAsync(0);

        return mockRepo;
    }

    public static Mock<IOrderRepository> CreateMockOrderRepository()
    {
        var mockRepo = new Mock<IOrderRepository>();

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order?)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order?)null);
        mockRepo.Setup(r => r.GetByMasterIdAsync(It.IsAny<int>())).ReturnsAsync(new List<Order>());

        return mockRepo;
    }

    public static Mock<IMasterOrderRepository> CreateMockMasterOrderRepository()
    {
        var mockRepo = new Mock<IMasterOrderRepository>();

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((MasterOrder?)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<MasterOrder>())).ReturnsAsync((MasterOrder?)null);
        mockRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(new List<MasterOrder>());
        mockRepo.Setup(r => r.GetWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((MasterOrder?)null);

        return mockRepo;
    }
}
