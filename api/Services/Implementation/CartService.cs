using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

/// <summary>
/// Cart service for authenticated users
/// Phase A.1: Removed apiKey parameter, now uses userId from JWT claims
/// </summary>
public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(string userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var cartItems = await _unitOfWork.Carts.GetByUserIdAsync(user.Usercode);
        
        // Map to CartItemDTO to avoid exposing Cart entity
        return cartItems.Select(c => new CartItemDTO
        {
            CartID = c.CartID,
            ItemID = c.ItemID,
            ItemName = c.ItemName,
            ItemPrice = c.ItemPrice,
            Quantity = c.Quantity,
            TotalPrice = c.ItemPrice * c.Quantity
        }).ToList();
    }

    public async Task<CartItemDTO> AddItemToCartAsync(string userId, SetCart setCart)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Fetch the Item by ID and validate it exists
        var item = await _unitOfWork.Items.GetByIdAsync(setCart.ItemID);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {setCart.ItemID} not found");

        var cart = new Cart
        {
            UserID = user.Usercode,
            ItemID = item.ItemID,
            ItemName = item.ItemName,
            ItemPrice = item.ItemPrice,
            Quantity = setCart.Quantity
        };

        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        return new CartItemDTO
        {
            CartID = cart.CartID,
            ItemID = cart.ItemID,
            ItemName = cart.ItemName,
            ItemPrice = cart.ItemPrice,
            Quantity = cart.Quantity,
            TotalPrice = cart.ItemPrice * cart.Quantity
        };
    }

    public async Task<bool> RemoveItemFromCartAsync(string userId, int itemId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return false;

        var removed = await _unitOfWork.Carts.RemoveByUserAndItemAsync(user.Usercode, itemId);
        if (removed)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return removed;
    }

    public async Task<CartDTO> GetCartSummaryAsync(string userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var cartItems = await _unitOfWork.Carts.GetByUserIdAsync(user.Usercode);
        
        var totalAmount = cartItems.Sum(c => c.ItemPrice * c.Quantity);
        
        return new CartDTO
        {
            cartitems = cartItems.Select(c => new CartItemDTO
            {
                CartID = c.CartID,
                ItemID = c.ItemID,
                ItemName = c.ItemName,
                ItemPrice = c.ItemPrice,
                Quantity = c.Quantity,
                TotalPrice = c.ItemPrice * c.Quantity
            }).ToList(),
            GrandTotal = totalAmount
        };
    }

    public async Task ClearCartAsync(string userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        await _unitOfWork.Carts.ClearByUserIdAsync(user.Usercode);
        await _unitOfWork.SaveChangesAsync();
    }
}