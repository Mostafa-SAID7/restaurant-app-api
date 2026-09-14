# Restaurant API Test Suite

Professional test suite for the Restaurant API built with .NET 8, xUnit, Moq, and FluentAssertions.

## Overview

This test suite provides comprehensive coverage of the Restaurant API's core business logic through unit and integration tests.

### Test Projects

- **RestaurantAPI.UnitTests** - Fast unit tests with mocked dependencies
- **RestaurantAPI.IntegrationTests** - API integration tests using WebApplicationFactory with InMemory database

### Test Structure

```
tests/
├── RestaurantAPI.UnitTests/
│   ├── TestHelpers/
│   │   ├── TestDataFactory.cs          # Test data builders
│   │   └── MockUnitOfWorkFactory.cs    # Mock repository setup
│   ├── Services/
│   │   ├── UserServiceTests.cs         # User registration, login, password
│   │   ├── OrderServiceTests.cs        # Order creation, validation, MasterID
│   │   └── CartServiceTests.cs         # Cart operations
│   ├── Helpers/
│   │   └── ValidationHelperTests.cs    # Validation rules
│   ├── Filters/
│   │   └── ApiKeyAuthorizationFilterTests.cs  # API key auth
│   └── Mapping/
│       └── MappingProfileTests.cs      # DTO/Entity mappings
├── RestaurantAPI.IntegrationTests/
│   ├── CustomWebApplicationFactory.cs  # Test server setup
│   ├── Auth/
│   │   └── UserAuthTests.cs            # Register, login, protected endpoints
│   ├── Orders/
│   │   └── OrderFlowTests.cs           # Order creation flow
│   └── Cart/
│       └── CartFlowTests.cs            # Cart operations
└── README.md                           # This file
```

## Running Tests

### All Tests

```bash
dotnet test
```

### Unit Tests Only

```bash
dotnet test tests/RestaurantAPI.UnitTests/
```

### Integration Tests Only

```bash
dotnet test tests/RestaurantAPI.IntegrationTests/
```

### Specific Test Class

```bash
dotnet test tests/RestaurantAPI.UnitTests/ -k UserServiceTests
```

### With Detailed Output

```bash
dotnet test --verbosity detailed
```

### Generate Coverage Report

```bash
dotnet test /p:CollectCoverage=true
```

## Test Coverage

### Unit Tests (60+ tests)

**UserService (10 tests)**
- ✅ User registration with password hashing
- ✅ Unique user code generation
- ✅ Login with valid/invalid credentials
- ✅ User deletion
- ✅ Password updates

**OrderService (8 tests)**
- ✅ Order creation with transaction flow
- ✅ MasterID from database (not predicted)
- ✅ Usercode consistency (not email)
- ✅ Empty order validation
- ✅ Item existence validation
- ✅ Server-side GrandTotal calculation
- ✅ Item price/name snapshotting

**CartService (8 tests)**
- ✅ Add/remove items
- ✅ Cart item totals
- ✅ Clear cart operations
- ✅ Cart summary with grand total

**ValidationHelper (25+ tests)**
- ✅ Email, password rules
- ✅ API key GUID format validation
- ✅ Quantity range validation
- ✅ Price validation
- ✅ URL validation
- ✅ Pagination bounds

**ApiKeyAuthorizationFilter (10 tests)**
- ✅ Missing API key rejection
- ✅ Invalid format rejection
- ✅ Valid key with user lookup
- ✅ Context storage

**MappingProfile (14 tests)**
- ✅ DTO/Entity conversions
- ✅ Password privacy in responses
- ✅ OrderLineDTO calculations
- ✅ CartItemDTO mappings
- ✅ Null handling

### Integration Tests (20+ tests)

**Auth Flow**
- ✅ Register → Login → Protected endpoint
- ✅ Invalid credentials rejection
- ✅ API key validation

**Order Flow**
- ✅ Order creation (happy path)
- ✅ Multiple items with correct total
- ✅ Unauthorized order rejection
- ✅ Retrieve user orders

**Cart Flow**
- ✅ Add/remove items
- ✅ Cart summary calculation
- ✅ Clear cart
- ✅ Multiple item operations

## Test Data Factory

Create consistent test data with fluent builders:

```csharp
var user = TestDataFactory.CreateUser(
    userCode: Guid.NewGuid().ToString(),
    email: "test@example.com"
);

var order = TestDataFactory.CreateOrder(
    itemName: "Pizza",
    itemPrice: 15.99m,
    quantity: 2
);
```

## Mock Setup

Pre-configured mocks for all repositories:

```csharp
var mockUnitOfWork = MockUnitOfWorkFactory.CreateMockUnitOfWork();

// Configure specific behavior
mockUnitOfWork.Setup(u => u.Users.GetByUserCodeAsync(apiKey))
    .ReturnsAsync(testUser);
```

## CI/CD Integration

Tests run automatically on:
- Push to main/master
- Pull requests to main/master

**CI Workflow** (`.github/workflows/ci.yml`):
1. Checkout code
2. Setup .NET 8
3. Restore dependencies
4. Build (Release)
5. Run Unit Tests
6. Run Integration Tests
7. Publish application
8. Verify Docker build

Tests must pass for the build to succeed. No `|| true` bypass - failures stop the pipeline.

## Key Testing Patterns

### Unit Test Template

```csharp
[Fact]
public async Task Method_Scenario_ExpectedResult()
{
    // Arrange - Setup test data and mocks
    var user = TestDataFactory.CreateUser();
    var mockRepo = MockUnitOfWorkFactory.CreateMockUnitOfWork();

    // Act - Execute the method
    var result = await service.MethodAsync(user);

    // Assert - Verify outcome using FluentAssertions
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
}
```

### Integration Test Template

```csharp
[Fact]
public async Task Endpoint_Scenario_ExpectedResult()
{
    // Arrange - Setup via API
    var client = _factory.CreateClient();
    var requestData = new { /* ... */ };

    // Act - Call endpoint
    var response = await client.PostAsync("/api/endpoint", content);

    // Assert - Verify response
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## Best Practices

### Unit Tests
- Mock external dependencies (databases, services)
- Test one logical concern per test
- Use descriptive test names: `Method_Scenario_ExpectedResult`
- Assert one primary outcome per test

### Integration Tests
- Use InMemory database for speed and isolation
- Test complete flows (register → login → order)
- Clean database state between tests
- Verify API contract (status codes, response shapes)

### General
- Keep tests fast (< 1ms for unit tests)
- Avoid test interdependencies
- Test edge cases and error paths
- Use FluentAssertions for readable assertions

## Troubleshooting

### Tests timeout
- Increase timeout: `dotnet test --logger "console;verbosity=detailed"`
- Check for infinite loops or blocking calls

### InMemory database issues
- Each test gets a unique InMemory database name
- No shared state between tests

### Auth failures in integration tests
- Verify X-API-Key header format (must be valid GUID)
- Check that RegisterAndGetUserCode helper is called first

## Future Enhancements

- [ ] Add E2E tests with real SQL Server
- [ ] Performance benchmarking tests
- [ ] Load testing scenarios
- [ ] API contract testing with Swagger
- [ ] Visual regression tests (if UI added)

## Contributing

When adding features:
1. Write tests first (TDD approach preferred)
2. Ensure all tests pass: `dotnet test`
3. Maintain >80% code coverage
4. Follow existing test patterns
5. Document new test helpers

## Resources

- [xUnit.net Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Documentation](https://fluentassertions.com/introduction)
- [Microsoft.AspNetCore.Mvc.Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
