using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Auth.Policies;
using RestaurantAPI.DTOs;
using RestaurantAPI.Filters;
using RestaurantAPI.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    [SwaggerTag("Shopping Cart Management")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;

        public CartController(ILogger<CartController> logger, ICartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        /// <summary>
        /// Get cart items for authenticated user
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get cart items", Description = "Retrieve all items in authenticated user's cart")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<CartDTO>))]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> GetCart()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var cartItems = await _cartService.GetCartItemsAsync(userId);
                return ResponseHelper.Success(cartItems);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Add item to authenticated user's cart
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Add item to cart", Description = "Add an item to authenticated user's cart")]
        [SwaggerResponse(201, "Item added to cart successfully", typeof(CartDTO))]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(404, "User or item not found")]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult> AddItemToCart([FromBody] AddCartItemRequestDTO addCartItem)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var cartDTO = await _cartService.AddItemToCartAsync(userId, addCartItem);
                return ResponseHelper.Created(cartDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Remove specific item from authenticated user's cart
        /// </summary>
        [HttpDelete("items/{itemId}")]
        [SwaggerOperation(Summary = "Remove item from cart", Description = "Remove a specific item from authenticated user's cart")]
        [SwaggerResponse(200, "Item removed successfully")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(404, "Item or user not found")]
        public async Task<ActionResult> RemoveItemFromCart(int itemId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var removed = await _cartService.RemoveItemFromCartAsync(userId, itemId);
                
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

        /// <summary>
        /// Get cart summary for authenticated user
        /// </summary>
        [HttpGet("summary")]
        [SwaggerOperation(Summary = "Get cart summary", Description = "Get cart items with total amount for authenticated user")]
        [SwaggerResponse(200, "Success", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> GetCartSummary()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var cartSummary = await _cartService.GetCartSummaryAsync(userId);
                return ResponseHelper.Success(cartSummary);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Clear all items from authenticated user's cart
        /// </summary>
        [HttpDelete]
        [SwaggerOperation(Summary = "Clear cart", Description = "Remove all items from authenticated user's cart")]
        [SwaggerResponse(200, "Cart cleared successfully")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                await _cartService.ClearCartAsync(userId);
                return ResponseHelper.Success<object>(null, "Cart cleared successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.NotFound(ex.Message);
            }
        }
    }
}
