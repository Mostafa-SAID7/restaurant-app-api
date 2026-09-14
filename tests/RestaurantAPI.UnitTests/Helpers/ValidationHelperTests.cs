using FluentAssertions;
using RestaurantAPI.Helpers;
using Xunit;

namespace RestaurantAPI.UnitTests.Helpers;

public class ValidationHelperTests
{
    #region ValidateUserRegistration Tests

    [Fact]
    public void ValidateUserRegistration_WithValidInput_ReturnsTrue()
    {
        var (isValid, errors) = ValidationHelper.ValidateUserRegistration("user@test.com", "Password123");

        isValid.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateUserRegistration_WithNullEmail_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateUserRegistration(null!, "Password123");

        isValid.Should().BeFalse();
        errors.Should().Contain("Email is required");
    }

    [Fact]
    public void ValidateUserRegistration_WithInvalidEmailFormat_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateUserRegistration("invalid-email", "Password123");

        isValid.Should().BeFalse();
        errors.Should().Contain("Invalid email format");
    }

    [Fact]
    public void ValidateUserRegistration_WithPasswordTooShort_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateUserRegistration("user@test.com", "Pass1");

        isValid.Should().BeFalse();
        errors.Should().Contain("Password must be at least 6 characters long");
    }

    [Fact]
    public void ValidateUserRegistration_WithPasswordTooLong_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateUserRegistration("user@test.com", "Password123LongPassword123456789");

        isValid.Should().BeFalse();
        errors.Should().Contain("Password cannot exceed 30 characters");
    }

    #endregion

    #region ValidatePassword Tests

    [Fact]
    public void ValidatePassword_WithValidPassword_ReturnsTrue()
    {
        var (isValid, errors) = ValidationHelper.ValidatePassword("NewPassword123");

        isValid.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidatePassword_WithPasswordTooShort_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidatePassword("Short");

        isValid.Should().BeFalse();
        errors.Should().Contain("Password must be at least 6 characters long");
    }

    #endregion

    #region ValidateMenuItem Tests

    [Fact]
    public void ValidateMenuItem_WithValidInput_ReturnsTrue()
    {
        var (isValid, errors) = ValidationHelper.ValidateMenuItem("Pasta", 15.99m, "Delicious pasta");

        isValid.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateMenuItem_WithNegativePrice_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateMenuItem("Item", -10.00m, null);

        isValid.Should().BeFalse();
        errors.Should().Contain("Item price must be greater than 0");
    }

    [Fact]
    public void ValidateMenuItem_WithZeroPrice_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateMenuItem("Item", 0m, null);

        isValid.Should().BeFalse();
        errors.Should().Contain("Item price must be greater than 0");
    }

    [Fact]
    public void ValidateMenuItem_WithPriceTooHigh_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateMenuItem("Item", 1000000m, null);

        isValid.Should().BeFalse();
        errors.Should().Contain("Item price is too high");
    }

    #endregion

    #region ValidateOrderQuantity Tests

    [Fact]
    public void ValidateOrderQuantity_WithValidQuantity_ReturnsTrue()
    {
        var (isValid, error) = ValidationHelper.ValidateOrderQuantity(5);

        isValid.Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public void ValidateOrderQuantity_WithZeroQuantity_ReturnsFalse()
    {
        var (isValid, error) = ValidationHelper.ValidateOrderQuantity(0);

        isValid.Should().BeFalse();
        error.Should().Contain("Quantity must be at least 1");
    }

    [Fact]
    public void ValidateOrderQuantity_WithNegativeQuantity_ReturnsFalse()
    {
        var (isValid, error) = ValidationHelper.ValidateOrderQuantity(-5);

        isValid.Should().BeFalse();
        error.Should().Contain("Quantity must be at least 1");
    }

    [Fact]
    public void ValidateOrderQuantity_WithQuantityAboveMax_ReturnsFalse()
    {
        var (isValid, error) = ValidationHelper.ValidateOrderQuantity(101);

        isValid.Should().BeFalse();
        error.Should().Contain("Quantity cannot exceed 100");
    }

    [Fact]
    public void ValidateOrderQuantity_WithMaxQuantity_ReturnsTrue()
    {
        var (isValid, error) = ValidationHelper.ValidateOrderQuantity(100);

        isValid.Should().BeTrue();
        error.Should().BeNull();
    }

    #endregion

    #region IsValidApiKey Tests

    [Fact]
    public void IsValidApiKey_WithValidGuid_ReturnsTrue()
    {
        var validGuid = Guid.NewGuid().ToString();
        var result = ValidationHelper.IsValidApiKey(validGuid);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidApiKey_WithInvalidFormat_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidApiKey("not-a-guid");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidApiKey_WithNullOrEmpty_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidApiKey(null);

        result.Should().BeFalse();
    }

    #endregion

    #region ValidatePagination Tests

    [Fact]
    public void ValidatePagination_WithValidInput_ReturnsCorrectValues()
    {
        var (page, pageSize) = ValidationHelper.ValidatePagination(2, 25);

        page.Should().Be(2);
        pageSize.Should().Be(25);
    }

    [Fact]
    public void ValidatePagination_WithNegativePage_ReturnPage1()
    {
        var (page, pageSize) = ValidationHelper.ValidatePagination(-1, 10);

        page.Should().Be(1);
    }

    [Fact]
    public void ValidatePagination_WithZeroPage_ReturnPage1()
    {
        var (page, pageSize) = ValidationHelper.ValidatePagination(0, 10);

        page.Should().Be(1);
    }

    [Fact]
    public void ValidatePagination_WithPageSizeAboveMax_CapsAt100()
    {
        var (page, pageSize) = ValidationHelper.ValidatePagination(1, 500);

        pageSize.Should().Be(100);
    }

    #endregion

    #region IsValidSortParameter Tests

    [Fact]
    public void IsValidSortParameter_WithValidField_ReturnsTrue()
    {
        var allowedFields = new[] { "name", "price", "created" };
        var result = ValidationHelper.IsValidSortParameter("price", allowedFields);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidSortParameter_WithInvalidField_ReturnsFalse()
    {
        var allowedFields = new[] { "name", "price" };
        var result = ValidationHelper.IsValidSortParameter("invalid", allowedFields);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidSortParameter_WithEmptySort_ReturnsTrue()
    {
        var allowedFields = new[] { "name", "price" };
        var result = ValidationHelper.IsValidSortParameter(null, allowedFields);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidSortParameter_IsCaseInsensitive()
    {
        var allowedFields = new[] { "Name", "Price" };
        var result = ValidationHelper.IsValidSortParameter("price", allowedFields);

        result.Should().BeTrue();
    }

    #endregion

    #region IsValidUrl Tests

    [Fact]
    public void IsValidUrl_WithValidHttpsUrl_ReturnsTrue()
    {
        var result = ValidationHelper.IsValidUrl("https://example.com/image.jpg");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidUrl_WithValidHttpUrl_ReturnsTrue()
    {
        var result = ValidationHelper.IsValidUrl("http://example.com");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidUrl_WithInvalidUrl_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidUrl("not-a-url");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidUrl_WithNullOrEmpty_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidUrl(null);

        result.Should().BeFalse();
    }

    #endregion

    #region IsValidPriceRange Tests

    [Fact]
    public void IsValidPriceRange_WithValidRange_ReturnsTrue()
    {
        var result = ValidationHelper.IsValidPriceRange(10.00m, 100.00m);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidPriceRange_WithNegativeMin_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidPriceRange(-10.00m, 100.00m);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidPriceRange_WithMinGreaterThanMax_ReturnsFalse()
    {
        var result = ValidationHelper.IsValidPriceRange(100.00m, 50.00m);

        result.Should().BeFalse();
    }

    #endregion

    #region ValidateRestaurant Tests

    [Fact]
    public void ValidateRestaurant_WithValidInput_ReturnsTrue()
    {
        var (isValid, errors) = ValidationHelper.ValidateRestaurant("My Restaurant", "123 Main St", "Fine Dining");

        isValid.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateRestaurant_WithMissingName_ReturnsFalse()
    {
        var (isValid, errors) = ValidationHelper.ValidateRestaurant("", "123 Main St", "Fine Dining");

        isValid.Should().BeFalse();
        errors.Should().Contain("Restaurant name is required");
    }

    #endregion
}
