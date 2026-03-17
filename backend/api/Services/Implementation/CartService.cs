using AutoMapper;
using FakeRestuarantAPI.Data;
using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FakeRestuarantAPI.Services.Implementation;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CartService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Cart>> GetCartItemsAsync(string apiKey)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        return await _context.cart
            .Where(c => c.UserID == apiKey)
            .ToListAsync();
    }

    public async Task<CartDTO> AddItemToCartAsync(string apiKey, setcart setCart)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        var cartDTO = _mapper.Map<CartDTO>(setCart);
        cartDTO.UserID = user.Usercode;

        // Convert CartDTO to Cart entity and save
        var cartItem = new Cart
        {
            UserID = cartDTO.UserID,
            ItemID = cartDTO.ItemID,
            ItemName = cartDTO.ItemName,
            Quantity = cartDTO.Quantity,
            ItemPrice = cartDTO.ItemPrice
        };

        await _context.cart.AddAsync(cartItem);
        await _context.SaveChangesAsync();

        return cartDTO;
    }

    public async Task<bool> RemoveItemFromCartAsync(string apiKey, int itemId)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        if (user == null)
            return false;

        var cartItem = await _context.cart
            .FirstOrDefaultAsync(c => c.UserID == apiKey && c.ItemID == itemId);

        if (cartItem == null)
            return false;

        _context.cart.Remove(cartItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<getcartDTO> GetCartSummaryAsync(string apiKey)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        var cartItems = await _context.cart
            .Where(c => c.UserID == apiKey)
            .ToListAsync();

        var grandTotal = cartItems.Sum(c => c.ItemPrice * c.Quantity);

        return new getcartDTO
        {
            cartitems = cartItems,
            GrandTotal = grandTotal
        };
    }

    public async Task ClearCartAsync(string apiKey)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        var cartItems = await _context.cart
            .Where(c => c.UserID == apiKey)
            .ToListAsync();

        _context.cart.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }
}