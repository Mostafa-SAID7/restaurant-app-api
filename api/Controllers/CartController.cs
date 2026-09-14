using RestaurantAPI.DTOs;
using RestaurantAPI.Filters;
using RestaurantAPI.Models;
using RestaurantAPI.Helpers;
using RestaurantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Shopping cart management endpoints")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;

        public CartController(ILogger<CartController> logger, ICartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        [HttpGet("{apikey}")]
        [SwaggerOperation(Summary = "Get cart items", Description = "Retrieve all items in user's cart")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> GetCart(string apikey)
        {
            try
            {
                var cartItems = await _cartService.GetCartItemsAsync(apikey);
                return ResponseHelper.Success(cartItems);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        [HttpPost("{apikey}")]
        [SwaggerOperation(Summary = "Add item to cart", Description = "Add an item to user's cart")]
        [SwaggerResponse(201, "Item added to cart successfully")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> AddItemToCart(string apikey, [FromBody] SetCart setCart)
        {
            try
            {
                var cartDTO = await _cartService.AddItemToCartAsync(apikey, setCart);
                return ResponseHelper.Created(cartDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        [HttpDelete("{apikey}/items/{itemId}")]
        [SwaggerOperation(Summary = "Remove item from cart", Description = "Remove a specific item from user's cart")]
        [SwaggerResponse(200, "Item removed successfully")]
        [SwaggerResponse(404, "Item or user not found")]
        public async Task<ActionResult> RemoveItemFromCart(string apikey, int itemId)
        {
            try
            {
                var removed = await _cartService.RemoveItemFromCartAsync(apikey, itemId);
                
                if (removed)
                {
                    return ResponseHelper.Success<object>(null, "Item removed from cart");
                }
                
                return ResponseHelper.NotFound("Item in cart");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        [HttpGet("{apikey}/summary")]
        [SwaggerOperation(Summary = "Get cart summary", Description = "Get cart items with total amount")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> GetCartSummary(string apikey)
        {
            try
            {
                var cartSummary = await _cartService.GetCartSummaryAsync(apikey);
                return ResponseHelper.Success(cartSummary);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        [HttpDelete("{apikey}")]
        [SwaggerOperation(Summary = "Clear cart", Description = "Remove all items from user's cart")]
        [SwaggerResponse(200, "Cart cleared successfully")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> ClearCart(string apikey)
        {
            try
            {
                await _cartService.ClearCartAsync(apikey);
                return ResponseHelper.Success<object>(null, "Cart cleared successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }
    }
}
