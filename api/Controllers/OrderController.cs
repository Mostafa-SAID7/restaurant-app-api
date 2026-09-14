using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Helpers;
using RestaurantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Order management endpoints")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost("{restaurantid}/makeorder")]
        [SwaggerOperation(Summary = "Create order", Description = "Create a new order for a specific restaurant")]
        [SwaggerResponse(201, "Order created successfully", typeof(OrderResponseDTO))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Invalid API key")]
        public async Task<ActionResult> CreateOrder(int restaurantid, [FromQuery] string apikey, [FromBody] MenuDTO menuDTO)
        {
            try
            {
                var orderResult = await _orderService.CreateOrderAsync(restaurantid, apikey, menuDTO);
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

        [HttpGet]
        [SwaggerOperation(Summary = "Get user orders", Description = "Retrieve all orders for a user")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(401, "Invalid API key")]
        public async Task<ActionResult> GetOrders([FromQuery] string apikey)
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync(apikey);
                return ResponseHelper.Success(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get orders by master ID", Description = "Retrieve orders by master order ID")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(401, "Invalid API key")]
        public async Task<ActionResult> GetOrdersByMasterId([FromQuery] string apikey, int id)
        {
            try
            {
                var orders = await _orderService.GetOrdersByMasterIdAsync(apikey, id);
                return ResponseHelper.Success(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResponseHelper.Unauthorized(ex.Message);
            }
        }

        [HttpDelete("{Order_id}")]
        [SwaggerOperation(Summary = "Delete order", Description = "Delete a specific order")]
        [SwaggerResponse(200, "Order deleted successfully")]
        [SwaggerResponse(400, "Order not found")]
        [SwaggerResponse(401, "Invalid API key")]
        public async Task<ActionResult> DeleteOrder([FromRoute] int Order_id, [FromQuery] string apikey)
        {
            try
            {
                var deleted = await _orderService.DeleteOrderAsync(Order_id, apikey);
                
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

        [HttpDelete("master/{master_id}")]
        [SwaggerOperation(Summary = "Delete master order", Description = "Delete a master order and all associated orders")]
        [SwaggerResponse(200, "Master order deleted successfully")]
        [SwaggerResponse(400, "Master order not found")]
        [SwaggerResponse(401, "Invalid API key")]
        public async Task<ActionResult> DeleteMasterOrder([FromRoute] int master_id, [FromQuery] string apikey)
        {
            try
            {
                var result = await _orderService.DeleteMasterOrderAsync(master_id, apikey);
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
