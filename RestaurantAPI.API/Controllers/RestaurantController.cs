using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Features.Restaurants.Commands;
using RestaurantAPI.Application.Features.Restaurants.Queries;
using RestaurantAPI.Auth.Policies;
using RestaurantAPI.Filters;
using RestaurantAPI.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Restaurant Management")]
    public class RestaurantController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RestaurantController> _logger;
        private readonly IImageService _imageService;

        public RestaurantController(
            IMediator mediator,
            ILogger<RestaurantController> logger,
            IImageService imageService)
        {
            _mediator = mediator;
            _logger = logger;
            _imageService = imageService;
        }

        /// <summary>
        /// Get all restaurants (public endpoint)
        /// Supports filtering by category, address, and name
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all restaurants", Description = "Retrieve all restaurants with optional filtering")]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<IEnumerable<RestaurantDto>>))]
        [SwaggerResponse(404, "No restaurants found")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RestaurantDto>>>> GetRestaurants(
            [FromQuery] string? category = null,
            [FromQuery] string? address = null,
            [FromQuery] string? name = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetRestaurantsQuery 
                { 
                    Category = category, 
                    Address = address, 
                    Name = name 
                };
                var restaurants = await _mediator.Send(query, cancellationToken);
                var restaurantList = restaurants.ToList();

                if (!restaurantList.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<RestaurantDto>>.CreateError("No restaurants found"));
                }

                return Ok(ApiResponse<IEnumerable<RestaurantDto>>.CreateSuccess(restaurantList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurants");
                return StatusCode(500, ApiResponse<IEnumerable<RestaurantDto>>.CreateError("An error occurred"));
            }
        }

        /// <summary>
        /// Create new restaurant (admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
        [SwaggerOperation(Summary = "Create restaurant", Description = "Add new restaurant (admin only)")]
        [SwaggerResponse(201, "Restaurant created successfully", typeof(ApiResponse<RestaurantDto>))]
        [SwaggerResponse(409, "Restaurant already exists")]
        [SwaggerResponse(403, "Forbidden - admin access required")]
        public async Task<ActionResult<ApiResponse<RestaurantDto>>> CreateRestaurant(
            [FromBody] CreateRestaurantDto restaurantData,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var command = new CreateRestaurantCommand { RestaurantData = restaurantData };
                var result = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetRestaurantById), new { restaurantId = result.RestaurantID }, 
                    ApiResponse<RestaurantDto>.CreateSuccess(result, "Restaurant created successfully"));
            }
            catch (ArgumentException ex)
            {
                return Conflict(ApiResponse<RestaurantDto>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating restaurant");
                return StatusCode(500, ApiResponse<RestaurantDto>.CreateError("An error occurred"));
            }
        }

        /// <summary>
        /// Get restaurant by ID (public endpoint)
        /// </summary>
        [HttpGet("{restaurantId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurant by ID", Description = "Retrieve specific restaurant details")]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<RestaurantDto>))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult<ApiResponse<RestaurantDto>>> GetRestaurantById(
            int restaurantId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetRestaurantByIdQuery { RestaurantId = restaurantId };
                var restaurant = await _mediator.Send(query, cancellationToken);
                
                if (restaurant == null)
                {
                    return NotFound(ApiResponse<RestaurantDto>.CreateError($"Restaurant with ID {restaurantId} not found"));
                }

                return Ok(ApiResponse<RestaurantDto>.CreateSuccess(restaurant));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurant");
                return StatusCode(500, ApiResponse<RestaurantDto>.CreateError("An error occurred"));
            }
        }

        /// <summary>
        /// Get restaurant menu (public endpoint)
        /// </summary>
        [HttpGet("{restaurantId}/menu")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurant menu", Description = "Retrieve menu items for specific restaurant")]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<IEnumerable<ItemResponseDto>>))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ItemResponseDto>>>> GetMenu(
            int restaurantId,
            [FromQuery] string sortByPrice = "",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetRestaurantMenuQuery 
                { 
                    RestaurantId = restaurantId,
                    SortByPrice = sortByPrice
                };
                var menu = await _mediator.Send(query, cancellationToken);
                var menuList = menu.ToList();

                if (!menuList.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<ItemResponseDto>>.CreateError($"No menu items found for restaurant {restaurantId}"));
                }

                return Ok(ApiResponse<IEnumerable<ItemResponseDto>>.CreateSuccess(menuList));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<IEnumerable<ItemResponseDto>>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurant menu");
                return StatusCode(500, ApiResponse<IEnumerable<ItemResponseDto>>.CreateError("An error occurred"));
            }
        }


        // TODO: Implement AddMenuItemCommand in Features/Restaurants/Commands
        // [HttpPost("{restaurantId}/items")]
        // [Authorize(Policy = AuthorizationPolicies.CanManageRestaurant)]
        // public async Task<ActionResult<ApiResponse<ItemDto>>> AddItemToMenu(...)

        /// <summary>
        /// Upload image file (authenticated, rate limited)
        /// </summary>
        [HttpPost("upload-image")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "Upload image file", Description = "Upload image for menu items")]
        [SwaggerResponse(200, "Image uploaded successfully", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Invalid image file")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(413, "File too large")]
        [RateLimit(maxRequests: 20, timeWindowMinutes: 1)]
        public async Task<ActionResult<ApiResponse<object>>> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.CreateError("No image file provided"));
                }

                if (!FileHelper.IsValidImage(image))
                {
                    return BadRequest(ApiResponse<object>.CreateError("Invalid image file. Allowed formats: JPEG, PNG, GIF, WebP. Max size: 5MB"));
                }

                var imagePath = await _imageService.SaveImageAsync(image);
                var imageUrl = _imageService.GetImageUrl(imagePath);

                return Ok(ApiResponse<object>.CreateSuccess(new { imagePath, imageUrl }, "Image uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, ApiResponse<object>.CreateError("An error occurred while uploading"));
            }
        }

        /// <summary>
        /// Upload base64 encoded image (authenticated, rate limited)
        /// </summary>
        [HttpPost("upload-base64-image")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "Upload base64 image", Description = "Upload image from base64 string")]
        [SwaggerResponse(200, "Image uploaded successfully", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Invalid image data")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(413, "File too large")]
        [RateLimit(maxRequests: 20, timeWindowMinutes: 1)]
        public async Task<ActionResult<ApiResponse<object>>> UploadBase64Image([FromBody] ImageRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Base64Image))
                {
                    return BadRequest(ApiResponse<object>.CreateError("No image data provided"));
                }

                // Validate base64 string
                try
                {
                    var base64Data = request.Base64Image.Contains(",") 
                        ? request.Base64Image.Split(',')[1] 
                        : request.Base64Image;
                    Convert.FromBase64String(base64Data);
                }
                catch
                {
                    return BadRequest(ApiResponse<object>.CreateError("Invalid base64 image data"));
                }

                // Sanitize filename
                var fileName = string.IsNullOrWhiteSpace(request.FileName) 
                    ? "item" 
                    : request.FileName.Replace("\\", "").Replace("/", "").Replace("..", "");

                var imagePath = await _imageService.SaveBase64ImageAsync(request.Base64Image, fileName);
                var imageUrl = _imageService.GetImageUrl(imagePath);

                return Ok(ApiResponse<object>.CreateSuccess(new { imagePath, imageUrl }, "Base64 image uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading base64 image");
                return StatusCode(500, ApiResponse<object>.CreateError("An error occurred while uploading"));
            }
        }

        /// <summary>
        /// Get all menu items (public endpoint)
        /// </summary>
        [HttpGet("items/all")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all menu items", Description = "Retrieve all menu items across all restaurants")]
        [SwaggerResponse(200, "Success", typeof(ApiResponse<IEnumerable<ItemResponseDto>>))]
        [SwaggerResponse(404, "No items found")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ItemResponseDto>>>> GetAllItems(
            [FromQuery] string itemName = "",
            [FromQuery] string sortByPrice = "",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetAllItemsQuery
                {
                    ItemName = itemName,
                    SortByPrice = sortByPrice
                };
                var items = await _mediator.Send(query, cancellationToken);
                var itemList = items.ToList();

                if (!itemList.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<ItemResponseDto>>.CreateError("No items found"));
                }

                return Ok(ApiResponse<IEnumerable<ItemResponseDto>>.CreateSuccess(itemList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return StatusCode(500, ApiResponse<IEnumerable<ItemResponseDto>>.CreateError("An error occurred"));
            }
        }
    }
}
