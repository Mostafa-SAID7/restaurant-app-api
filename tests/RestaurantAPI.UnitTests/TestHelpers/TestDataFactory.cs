using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.UnitTests.TestHelpers;

/// <summary>
/// Factory for creating test data entities with realistic values
/// </summary>
public static class TestDataFactory
{
    private static int _userCounter = 1;
    private static int _restaurantCounter = 1;
    private static int _itemCounter = 1;
    private static int _cartCounter = 1;
    private static int _masterOrderCounter = 1;
    private static int _orderCounter = 1;

    public static void Reset()
    {
        _userCounter = 1;
        _restaurantCounter = 1;
        _itemCounter = 1;
        _cartCounter = 1;
        _masterOrderCounter = 1;
        _orderCounter = 1;
    }

    public static User CreateUser(string? userCode = null, string? email = null, string? passwordHash = null)
    {
        return new User
        {
            Usercode = userCode ?? Guid.NewGuid().ToString(),
            UserEmail = email ?? $"user{_userCounter}@test.com",
            PasswordHash = passwordHash ?? "hashed_password_123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static Restaurant CreateRestaurant(int? restaurantId = null, string? name = null)
    {
        return new Restaurant
        {
            RestaurantID = restaurantId ?? _restaurantCounter++,
            RestaurantName = name ?? $"Restaurant {_restaurantCounter}",
            Address = $"Address {_restaurantCounter}",
            Type = "Fine Dining",
            ParkingLot = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static Item CreateItem(int? itemId = null, int? restaurantId = null, string? itemName = null, decimal? price = null)
    {
        return new Item
        {
            ItemID = itemId ?? _itemCounter++,
            ItemName = itemName ?? $"Item {_itemCounter}",
            ItemDescription = "Test item description",
            ItemPrice = price ?? 10.99m,
            RestaurantID = restaurantId ?? 1,
            Restaurant = CreateRestaurant(restaurantId),
            ImageUrl = "http://example.com/image.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static Cart CreateCart(int? cartId = null, string? userCode = null, int? itemId = null, int quantity = 1, decimal? itemPrice = null)
    {
        return new Cart
        {
            CartID = cartId ?? _cartCounter++,
            UserID = userCode ?? Guid.NewGuid().ToString(),
            ItemID = itemId ?? 1,
            ItemName = $"Item {itemId ?? 1}",
            ItemPrice = itemPrice ?? 15.50m,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static MasterOrder CreateMasterOrder(int? masterId = null, string? userCode = null, int? restaurantId = null, decimal? grandTotal = null)
    {
        return new MasterOrder
        {
            MasterID = masterId ?? _masterOrderCounter++,
            UserID = userCode ?? Guid.NewGuid().ToString(),
            RestaurantID = restaurantId ?? 1,
            GrandTotal = grandTotal ?? 50.00m,
            User = CreateUser(userCode),
            Restaurant = CreateRestaurant(restaurantId),
            Orders = new List<Order>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static Order CreateOrder(int? orderId = null, int? masterId = null, int? itemId = null, string? itemName = null, decimal? itemPrice = null, int quantity = 1)
    {
        return new Order
        {
            OrderID = orderId ?? _orderCounter++,
            MasterID = masterId ?? 1,
            ItemID = itemId ?? 1,
            ItemName = itemName ?? "Test Item",
            ItemPrice = itemPrice ?? 20.00m,
            Quantity = quantity,
            TotalPrice = (itemPrice ?? 20.00m) * quantity,
            UserID = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ResetCounters()
    {
        Reset();
    }
}
