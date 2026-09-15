using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Features.Orders.Commands;
using RestaurantAPI.Application.Features.Orders.Queries;
using RestaurantAPI.Auth.Policies;
using RestaurantAPI.Auth.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
[SwaggerTag("Order Management")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public OrderController(
        IMediator mediator,
        ILogger<OrderController> logger,
        ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    [HttpPost("{restaurantId}/create")]
    [SwaggerOperation(Summary = "Create order")]
    public async Task<ActionResult<ApiResponse<CreateOrderResponseDto>>> CreateOrder(
        int restaurantId,
        [FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<CreateOrderResponseDto>.CreateError("User not identified"));

            var command = new CreateOrderCommand
            {
                RestaurantId = restaurantId,
                UserId = userId,
                OrderData = createOrderDto
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrderByMasterId), 
                new { masterId = result.MasterID }, 
                ApiResponse<CreateOrderResponseDto>.CreateSuccess(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<CreateOrderResponseDto>.CreateError(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<CreateOrderResponseDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, ApiResponse<CreateOrderResponseDto>.CreateError("An error occurred"));
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get user orders")]
    public async Task<ActionResult<ApiResponse<GetUserOrdersResponse>>> GetUserOrders()
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<GetUserOrdersResponse>.CreateError("User not identified"));

            var query = new GetUserOrdersQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<GetUserOrdersResponse>.CreateSuccess(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(ApiResponse<GetUserOrdersResponse>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, ApiResponse<GetUserOrdersResponse>.CreateError("An error occurred"));
        }
    }

    [HttpGet("{masterId}")]
    [SwaggerOperation(Summary = "Get order lines")]
    public async Task<ActionResult<ApiResponse<List<OrderLineDto>>>> GetOrderByMasterId(int masterId)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<List<OrderLineDto>>.CreateError("User not identified"));

            var query = new GetOrderByMasterIdQuery { UserId = userId, MasterId = masterId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<List<OrderLineDto>>.CreateSuccess(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return StatusCode(500, ApiResponse<List<OrderLineDto>>.CreateError("An error occurred"));
        }
    }

    [HttpDelete("{orderId}")]
    [SwaggerOperation(Summary = "Delete order line")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteOrder(int orderId)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<bool>.CreateError("User not identified"));

            var command = new DeleteOrderCommand { OrderId = orderId, UserId = userId };
            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<bool>.CreateSuccess(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order");
            return StatusCode(500, ApiResponse<bool>.CreateError("An error occurred"));
        }
    }

    [HttpDelete("master/{masterId}")]
    [SwaggerOperation(Summary = "Delete master order")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMasterOrder(int masterId)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<bool>.CreateError("User not identified"));

            var command = new DeleteMasterOrderCommand { MasterId = masterId, UserId = userId };
            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<bool>.CreateSuccess(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<bool>.CreateError(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<bool>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting master order");
            return StatusCode(500, ApiResponse<bool>.CreateError("An error occurred"));
        }
    }
}
