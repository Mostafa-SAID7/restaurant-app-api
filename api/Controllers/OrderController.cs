using RestuarantAPI.Models;
using RestuarantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace RestuarantAPI.Controllers
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
        [SwaggerResponse(201, "Order created successfully", typeof(FullOrderDTO))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Invalid API key")]
        public async Task<ActionResult> CreateOrder(int restaurantid, [FromQuery] string apikey, [FromBody] MenuDTO menuDTO)
        {
            try
            {
                var orderResult = await _orderService.CreateOrderAsync(restaurantid, apikey, menuDTO);
                return StatusCode(201, orderResult);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "Internal server error");
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
                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return StatusCode(500, "Internal server error");
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
                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders by master ID");
                return StatusCode(500, "Internal server error");
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
                    return Ok(new { message = "Order deleted successfully" });
                }
                
                return StatusCode(400, new { message = "Order not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");
                return StatusCode(500, "Internal server error");
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
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting master order");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
