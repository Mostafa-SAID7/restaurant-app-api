using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(string apiKey)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

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

    public async Task<CartItemDTO> AddItemToCartAsync(string apiKey, SetCart setCart)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        var cart = new Cart
        {
            UserID = user.Usercode,
            ItemID = setCart.item.ItemID,
            ItemName = setCart.item.ItemName,
            ItemPrice = setCart.item.ItemPrice,
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

    public async Task<bool> RemoveItemFromCartAsync(string apiKey, int itemId)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            return false;

        var removed = await _unitOfWork.Carts.RemoveByUserAndItemAsync(user.Usercode, itemId);
        if (removed)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return removed;
    }

    public async Task<GetCartDTO> GetCartSummaryAsync(string apiKey)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        var cartItems = await _unitOfWork.Carts.GetByUserIdAsync(user.Usercode);
        
        var totalAmount = cartItems.Sum(c => c.ItemPrice * c.Quantity);
        
        return new GetCartDTO
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

    public async Task ClearCartAsync(string apiKey)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        await _unitOfWork.Carts.ClearByUserIdAsync(user.Usercode);
        await _unitOfWork.SaveChangesAsync();
    }
}