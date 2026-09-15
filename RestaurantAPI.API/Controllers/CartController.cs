using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Features.Cart.Commands;
using RestaurantAPI.Application.Features.Cart.Queries;
using RestaurantAPI.Auth.Policies;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace RestaurantAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
[SwaggerTag("Shopping Cart Management")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartController> _logger;

    public CartController(IMediator mediator, ILogger<CartController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get cart items")]
    public async Task<ActionResult<ApiResponse<List<CartItemDto>>>> GetCart()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<List<CartItemDto>>.CreateError("User not identified"));

            var query = new GetCartItemsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<List<CartItemDto>>.CreateSuccess(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(ApiResponse<List<CartItemDto>>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart");
            return StatusCode(500, ApiResponse<List<CartItemDto>>.CreateError("An error occurred"));
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Add item to cart")]
    public async Task<ActionResult<ApiResponse<CartItemDto>>> AddItemToCart([FromBody] AddCartItemDto addCartItem)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<CartItemDto>.CreateError("User not identified"));

            var command = new AddItemToCartCommand { UserId = userId, CartItem = addCartItem };
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetCart), ApiResponse<CartItemDto>.CreateSuccess(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(ApiResponse<CartItemDto>.CreateError(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CartItemDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return StatusCode(500, ApiResponse<CartItemDto>.CreateError("An error occurred"));
        }
    }

    [HttpDelete("{itemId}")]
    [SwaggerOperation(Summary = "Remove item from cart")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveItemFromCart(int itemId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<bool>.CreateError("User not identified"));

            var command = new RemoveItemFromCartCommand { UserId = userId, ItemId = itemId };
            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<bool>.CreateSuccess(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart");
            return StatusCode(500, ApiResponse<bool>.CreateError("An error occurred"));
        }
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Clear cart")]
    public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<bool>.CreateError("User not identified"));

            var command = new ClearCartCommand { UserId = userId };
            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<bool>.CreateSuccess(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(ApiResponse<bool>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, ApiResponse<bool>.CreateError("An error occurred"));
        }
    }

    [HttpGet("summary")]
    [SwaggerOperation(Summary = "Get cart summary")]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCartSummary()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<CartDto>.CreateError("User not identified"));

            var query = new GetCartSummaryQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<CartDto>.CreateSuccess(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(ApiResponse<CartDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart summary");
            return StatusCode(500, ApiResponse<CartDto>.CreateError("An error occurred"));
        }
    }
}
