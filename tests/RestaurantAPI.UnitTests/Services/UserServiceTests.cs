using AutoMapper;
using FluentAssertions;
using Moq;
using RestaurantAPI.Auth.Services.Interfaces;
using RestaurantAPI.DTOs;
using RestaurantAPI.Mapping;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Implementation;
using RestaurantAPI.UnitTests.TestHelpers;
using Xunit;

namespace RestaurantAPI.UnitTests.Services;

/// <summary>
/// Tests for UserService
/// Phase A.1-A.2: Tests for removed API-key and password methods have been deleted
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUnitOfWork = MockUnitOfWorkFactory.CreateMockUnitOfWork();
        _mockPasswordService = new Mock<IPasswordService>();
        
        // Setup AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _userService = new UserService(_mockUnitOfWork.Object, _mapper, _mockPasswordService.Object);
    }

    #region RegisterUserAsync Tests

    [Fact]
    public async Task RegisterUserAsync_WithValidUserDTO_CreatesUserWithHashedPassword()
    {
        // Arrange
        var userDto = new UserDTO
        {
            UserEmail = "newuser@test.com",
            Password = "SecurePassword123!"
        };

        var hashedPassword = "hashed_password_from_service";
        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns(hashedPassword);

        User? capturedUser = null;
        _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync(capturedUser);

        // Act
        var result = await _userService.RegisterUserAsync(userDto);

        // Assert
        result.Should().NotBeNull();
        result.UserEmail.Should().Be(userDto.UserEmail);
        result.Usercode.Should().NotBeNullOrEmpty();
        result.PasswordHash.Should().Be(hashedPassword);
        capturedUser.Should().NotBeNull();
        _mockPasswordService.Verify(p => p.HashPassword(userDto.Password), Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_GeneratesUniqueUserCode()
    {
        // Arrange
        var userDto = new UserDTO
        {
            UserEmail = "user@test.com",
            Password = "Password123!"
        };

        var hashedPassword = "hashed_password";
        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns(hashedPassword);

        var generatedCodes = new List<string>();
        _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => generatedCodes.Add(u.Usercode))
            .ReturnsAsync((User?)null);

        // Act
        var user1 = await _userService.RegisterUserAsync(userDto);
        var user2 = await _userService.RegisterUserAsync(userDto);

        // Assert
        user1.Usercode.Should().NotBe(user2.Usercode);
        generatedCodes.Distinct().Should().HaveCount(2);
    }

    [Fact]
    public async Task RegisterUserAsync_SetsCreatedAtToUtcNow()
    {
        // Arrange
        var userDto = new UserDTO
        {
            UserEmail = "user@test.com",
            Password = "Password123!"
        };

        var hashedPassword = "hashed_password";
        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns(hashedPassword);

        var beforeRegistration = DateTime.UtcNow;
        User? capturedUser = null;

        _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User?)null);

        // Act
        await _userService.RegisterUserAsync(userDto);
        var afterRegistration = DateTime.UtcNow;

        // Assert
        capturedUser?.CreatedAt.Should().BeOnOrAfter(beforeRegistration);
        capturedUser?.CreatedAt.Should().BeOnOrBefore(afterRegistration);
    }

    #endregion

    #region UserExistsAsync Tests

    [Fact]
    public async Task UserExistsAsync_WhenEmailExists_ReturnsTrue()
    {
        // Arrange
        var email = "existing@test.com";
        _mockUnitOfWork.Setup(u => u.Users.EmailExistsAsync(email))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UserExistsAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserExistsAsync_WhenEmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = "nonexistent@test.com";
        _mockUnitOfWork.Setup(u => u.Users.EmailExistsAsync(email))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.UserExistsAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region DeleteUserAsync Tests

    [Fact]
    public async Task DeleteUserAsync_WithValidUserId_DeletesUserAndReturnsTrue()
    {
        // Arrange
        var userId = "user123";
        var user = TestDataFactory.CreateUser();

        _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Users.DeleteAsync(user))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        result.Should().BeTrue();
        _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithInvalidUserId_ReturnsFalse()
    {
        // Arrange
        var invalidUserId = "invalid_user";
        _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(invalidUserId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteUserAsync(invalidUserId);

        // Assert
        result.Should().BeFalse();
        _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region GetUserByIdAsync Tests

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var userId = "user123";
        var user = TestDataFactory.CreateUser();

        _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(user);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidUserId = "invalid_user";
        _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(invalidUserId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(invalidUserId);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
