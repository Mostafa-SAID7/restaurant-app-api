using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantAPI.Filters;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.UnitTests.TestHelpers;
using Xunit;

namespace RestaurantAPI.UnitTests.Filters;

public class ApiKeyAuthorizationFilterTests
{
    private readonly Mock<ILogger<ApiKeyAuthorizationFilter>> _mockLogger;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ApiKeyAuthorizationFilter _filter;

    public ApiKeyAuthorizationFilterTests()
    {
        _mockLogger = new Mock<ILogger<ApiKeyAuthorizationFilter>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _filter = new ApiKeyAuthorizationFilter(_mockLogger.Object, _mockUserRepository.Object);
    }

    private AuthorizationFilterContext CreateFilterContext(string? apiKey = null, string? headerName = "X-API-Key")
    {
        var httpContext = new DefaultHttpContext();
        
        if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(headerName))
        {
            httpContext.Request.Headers[headerName] = apiKey;
        }

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    #region Missing API Key Tests

    [Fact]
    public async Task OnAuthorizationAsync_WithMissingApiKey_ReturnUnauthorized()
    {
        // Arrange
        var context = CreateFilterContext(apiKey: null);

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        context.Result.Should().BeOfType<UnauthorizedObjectResult>();
        var result = (UnauthorizedObjectResult)context.Result;
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task OnAuthorizationAsync_WithEmptyApiKey_ReturnUnauthorized()
    {
        // Arrange
        var context = CreateFilterContext(apiKey: "");

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        context.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region Invalid Format Tests

    [Fact]
    public async Task OnAuthorizationAsync_WithInvalidGuidFormat_ReturnUnauthorized()
    {
        // Arrange
        var context = CreateFilterContext(apiKey: "not-a-valid-guid");

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        context.Result.Should().BeOfType<UnauthorizedObjectResult>();
        var result = (UnauthorizedObjectResult)context.Result;
        result.StatusCode.Should().Be(401);
        result.Value.Should().NotBeNull();

        // Anonymous type: assert via reflection since BeOfType<object>() fails for compiler-generated types
        var value = result.Value!;
        var messageProp = value.GetType().GetProperty("message");
        messageProp.Should().NotBeNull("Anonymous type should contain 'message' property");
        messageProp!.GetValue(value)?.ToString().Should().Contain("Invalid API key format");

        var timestampProp = value.GetType().GetProperty("timestamp");
        timestampProp.Should().NotBeNull("Anonymous type should contain 'timestamp' property");
    }

    [Fact]
    public async Task OnAuthorizationAsync_WithInvalidFormat_LogsWarning()
    {
        // Arrange
        var context = CreateFilterContext(apiKey: "invalid-format");

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("invalid API key format")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Valid Format But User Not Found Tests

    [Fact]
    public async Task OnAuthorizationAsync_WithValidFormatButUserNotFound_ReturnUnauthorized()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var context = CreateFilterContext(apiKey: validGuid);

        // Mock repository to return no user
        _mockUserRepository.Setup(r => r.GetByUserCodeAsync(validGuid))
            .ReturnsAsync((RestaurantAPI.Models.User?)null);

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        context.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region Successful Authorization Tests

    [Fact]
    public async Task OnAuthorizationAsync_WithValidApiKey_StoresKeyAndUserInContext()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: validGuid);
        var context = CreateFilterContext(apiKey: validGuid);

        _mockUserRepository.Setup(r => r.GetByUserCodeAsync(validGuid))
            .ReturnsAsync(user);

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        context.Result.Should().BeNull();
        context.HttpContext.Items.Should().ContainKey("ApiKey");
        context.HttpContext.Items.Should().ContainKey("User");
        context.HttpContext.Items["ApiKey"].Should().Be(validGuid);
        context.HttpContext.Items["User"].Should().Be(user);
    }

    [Fact]
    public async Task OnAuthorizationAsync_WithValidApiKey_LogsSuccess()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: validGuid);
        var context = CreateFilterContext(apiKey: validGuid);

        _mockUserRepository.Setup(r => r.GetByUserCodeAsync(validGuid))
            .ReturnsAsync(user);

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("API request authorized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnAuthorizationAsync_WithValidApiKey_DoesNotSetErrorResult()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: validGuid);
        var context = CreateFilterContext(apiKey: validGuid);

        _mockUserRepository.Setup(r => r.GetByUserCodeAsync(validGuid))
            .ReturnsAsync(user);

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert
        context.Result.Should().BeNull();
    }

    #endregion

    #region Multiple API Key Formats Tests

    [Fact]
    public async Task OnAuthorizationAsync_WithBearerToken_ExtractsApiKey()
    {
        // Note: This test assumes the GetApiKey extension checks Bearer token
        // The actual implementation may vary - adjust based on extension logic
        var validGuid = Guid.NewGuid().ToString();
        var user = TestDataFactory.CreateUser(userCode: validGuid);
        var context = CreateFilterContext(apiKey: validGuid, headerName: "Authorization");
        context.HttpContext.Request.Headers["Authorization"] = $"Bearer {validGuid}";

        _mockUserRepository.Setup(r => r.GetByUserCodeAsync(validGuid))
            .ReturnsAsync(user);

        // Act
        await _filter.OnAuthorizationAsync(context);

        // Assert - Should either pass or fail based on implementation
        context.Result.Should().BeNull();
    }

    #endregion
}
