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

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUnitOfWork = MockUnitOfWorkFactory.CreateMockUnitOfWork();
        
        // Setup AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _userService = new UserService(_mockUnitOfWork.Object, _mapper);
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

        User? capturedUser = null;
        _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync(capturedUser);

        _mockUnitOfWork.Setup(u => u.Users.UserCodeExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.RegisterUserAsync(userDto);

        // Assert
        result.Should().NotBeNull();
        result.UserEmail.Should().Be(userDto.UserEmail);
        result.Usercode.Should().NotBeNullOrEmpty();
        result.PasswordHash.Should().NotBe(userDto.Password);
        capturedUser.Should().NotBeNull();
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

        var generatedCodes = new List<string>();
        _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => generatedCodes.Add(u.Usercode))
            .ReturnsAsync((User?)null);

        _mockUnitOfWork.Setup(u => u.Users.UserCodeExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

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

        var beforeRegistration = DateTime.UtcNow;
        User? capturedUser = null;

        _mockUnitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User?)null);

        _mockUnitOfWork.Setup(u => u.Users.UserCodeExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

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

    #region GetUserCodeAsync Tests

    [Fact]
    public async Task GetUserCodeAsync_WithValidCredentials_ReturnsUserCode()
    {
        // Arrange
        var email = "user@test.com";
        var password = "ValidPassword123!";
        var userCode = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: userCode, email: email);

        _mockUnitOfWork.Setup(u => u.Users.ValidateUserAsync(email, password))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserCodeAsync(email, password);

        // Assert
        result.Should().Be(userCode);
    }

    [Fact]
    public async Task GetUserCodeAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var email = "user@test.com";
        var password = "WrongPassword";

        _mockUnitOfWork.Setup(u => u.Users.ValidateUserAsync(email, password))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserCodeAsync(email, password);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetUserByCodeAsync Tests

    [Fact]
    public async Task GetUserByCodeAsync_WithValidCode_ReturnsUser()
    {
        // Arrange
        var userCode = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: userCode);

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(userCode))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByCodeAsync(userCode);

        // Assert
        result.Should().NotBeNull();
        result?.Usercode.Should().Be(userCode);
    }

    [Fact]
    public async Task GetUserByCodeAsync_WithInvalidCode_ReturnsNull()
    {
        // Arrange
        var invalidCode = Guid.NewGuid().ToString();
        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(invalidCode))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByCodeAsync(invalidCode);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteUserAsync Tests

    [Fact]
    public async Task DeleteUserAsync_WithValidApiKey_DeletesUserAndReturnsTrue()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Users.DeleteAsync(user))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(apiKey);

        // Assert
        result.Should().BeTrue();
        _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithInvalidApiKey_ReturnsFalse()
    {
        // Arrange
        var invalidKey = Guid.NewGuid().ToString();
        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(invalidKey))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteUserAsync(invalidKey);

        // Assert
        result.Should().BeFalse();
        _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region UpdateUserPasswordAsync Tests

    [Fact]
    public async Task UpdateUserPasswordAsync_WithValidApiKey_UpdatesPasswordAndReturnsUser()
    {
        // Arrange
        var apiKey = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: apiKey);
        var originalPassword = user.PasswordHash;
        var newPassword = "NewSecurePassword123!";

        User? capturedUser = null;
        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(u => u.Users.UpdateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUserPasswordAsync(apiKey, newPassword);

        // Assert
        result.Should().NotBeNull();
        capturedUser?.PasswordHash.Should().NotBe(originalPassword);
        _mockUnitOfWork.Verify(u => u.Users.UpdateAsync(It.IsAny<User>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_WithInvalidApiKey_ReturnsNull()
    {
        // Arrange
        var invalidKey = Guid.NewGuid().ToString();
        var newPassword = "NewPassword123!";

        _mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(invalidKey))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserPasswordAsync(invalidKey, newPassword);

        // Assert
        result.Should().BeNull();
        _mockUnitOfWork.Verify(u => u.Users.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion
}
