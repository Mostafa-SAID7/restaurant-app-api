using AutoMapper;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using RestuarantAPI.Services.Interfaces;

namespace RestuarantAPI.Services.Implementation;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Cart>> GetCartItemsAsync(string apiKey)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        return await _unitOfWork.Carts.GetByUserIdAsync(user.Usercode);
    }

    public async Task<CartDTO> AddItemToCartAsync(string apiKey, SetCart setCart)
    {
        var user = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (user == null)
            throw new UnauthorizedAccessException("No user found with given key");

        var cartDTO = _mapper.Map<CartDTO>(setCart);
        cartDTO.UserID = user.Usercode;

        var cart = _mapper.Map<Cart>(cartDTO);
        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        return cartDTO;
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
        
        var totalAmount = cartItems.Sum(c => c.item.ItemPrice * c.Quantity);
        
        return new GetCartDTO
        {
            cartitems = cartItems.ToList(),
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