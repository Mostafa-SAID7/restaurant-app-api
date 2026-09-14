using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Auth.Policies;
using RestaurantAPI.DTOs;
using RestaurantAPI.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    [SwaggerTag("Order Management")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        /// <summary>
        /// Create order for specific restaurant
        /// </summary>
        [HttpPost("{restaurantId}/create")]
        [SwaggerOperation(Summary = "Create order", Description = "Create a new order for a specific restaurant")]
        [SwaggerResponse(201, "Order created successfully", typeof(OrderResponseDTO))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        public async Task<ActionResult> CreateOrder(int restaurantId, [FromBody] CreateOrderRequestDTO createOrderRequest)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var orderResult = await _orderService.CreateOrderAsync(restaurantId, userId, createOrderRequest);
                return ResponseHelper.Created(orderResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return ResponseHelper.Error(ex.Message);
            }
        }

        /// <summary>
        /// Get all orders for authenticated user
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get user orders", Description = "Retrieve all orders for authenticated user")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<OrderResponseDTO>))]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        public async Task<ActionResult> GetOrders()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var orders = await _orderService.GetUserOrdersAsync(userId);
                return ResponseHelper.Success(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Get orders by master order ID
        /// </summary>
        [HttpGet("master/{masterId}")]
        [SwaggerOperation(Summary = "Get orders by master ID", Description = "Retrieve orders by master order ID")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<OrderResponseDTO>))]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        public async Task<ActionResult> GetOrdersByMasterId(int masterId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var orders = await _orderService.GetOrdersByMasterIdAsync(userId, masterId);
                return ResponseHelper.Success(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Delete specific order
        /// </summary>
        [HttpDelete("{orderId}")]
        [SwaggerOperation(Summary = "Delete order", Description = "Delete a specific order (user must own the order)")]
        [SwaggerResponse(200, "Order deleted successfully")]
        [SwaggerResponse(400, "Order not found")]
        [SwaggerResponse(401, "Unauthorized - JWT token required or order not owned by user")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var deleted = await _orderService.DeleteOrderAsync(orderId, userId);
                
                if (deleted)
                {
                    return ResponseHelper.Success<object>(null, "Order deleted successfully");
                }
                
                return ResponseHelper.Error("Order not found", 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Delete master order and all associated orders
        /// </summary>
        [HttpDelete("master/{masterId}")]
        [SwaggerOperation(Summary = "Delete master order", Description = "Delete a master order and all associated orders")]
        [SwaggerResponse(200, "Master order deleted successfully")]
        [SwaggerResponse(400, "Master order not found")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        public async Task<ActionResult> DeleteMasterOrder(int masterId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return ResponseHelper.Unauthorized("User not identified from JWT");
                }

                var result = await _orderService.DeleteMasterOrderAsync(masterId, userId);
                return ResponseHelper.Success(result, "Master order deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return ResponseHelper.Error(ex.Message, 400);
            }
        }
    }
}
