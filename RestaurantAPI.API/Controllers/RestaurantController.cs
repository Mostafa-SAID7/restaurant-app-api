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
    [SwaggerTag("Restaurant Management")]
    public class RestaurantController : ControllerBase
    {
        private readonly ILogger<RestaurantController> _logger;
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;
        private readonly IItemService _itemService;
        private readonly IImageService _imageService;

        public RestaurantController(
            ILogger<RestaurantController> logger, 
            IRestaurantService restaurantService,
            IMenuService menuService,
            IItemService itemService,
            IImageService imageService)
        {
            _logger = logger;
            _restaurantService = restaurantService;
            _menuService = menuService;
            _itemService = itemService;
            _imageService = imageService;
        }

        /// <summary>
        /// Get all restaurants (public endpoint)
        /// Phase B.4: Supports pagination via pageNumber and pageSize query parameters
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all restaurants", Description = "Retrieve all restaurants with optional filtering and pagination")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<RestaurantDTO>))]
        [SwaggerResponse(404, "No restaurants found")]
        public async Task<ActionResult> GetRestaurants(
            [FromQuery] string category = "", 
            [FromQuery] string? address = null, 
            [FromQuery] string? name = null,
            [FromQuery(Name = "pageNumber")] string? pageNumberStr = null,
            [FromQuery(Name = "pageSize")] string? pageSizeStr = null)
        {
            // Extract and validate pagination parameters
            var paginationParams = PaginationHelper.ExtractFromQuery(pageNumberStr, pageSizeStr);
            
            var restaurants = await _restaurantService.GetRestaurantsAsync(category, address, name);
            var restaurantList = restaurants.ToList();
            
            if (!restaurantList.Any())
            {
                return ResponseHelper.NotFound("Restaurants");
            }

            // Apply pagination manually (until service layer is updated)
            var totalCount = restaurantList.Count;
            var paginatedRestaurants = restaurantList
                .Skip(paginationParams.GetOffset())
                .Take(paginationParams.PageSize)
                .ToList();

            var paginatedResponse = PaginatedResponse<RestaurantDTO>.Create(
                paginatedRestaurants, 
                paginationParams.PageNumber, 
                paginationParams.PageSize, 
                totalCount);

            return ResponseHelper.PaginatedStandard(paginatedResponse);
        }

        /// <summary>
        /// Create new restaurant (admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
        [SwaggerOperation(Summary = "Create restaurant", Description = "Add new restaurant (admin only)")]
        [SwaggerResponse(201, "Restaurant created successfully", typeof(RestaurantDTO))]
        [SwaggerResponse(409, "Restaurant already exists")]
        [SwaggerResponse(403, "Forbidden - admin access required")]
        public async Task<ActionResult> CreateRestaurant(RestaurantDTO restaurantDTO)
        {
            var restaurantExists = await _restaurantService.RestaurantExistsAsync(restaurantDTO.RestaurantName);
            if (restaurantExists)
            {
                return ResponseHelper.Error("Restaurant already exists", 409);
            }

            var newRestaurant = await _restaurantService.CreateRestaurantAsync(restaurantDTO);
            return ResponseHelper.Created(newRestaurant);
        }

        /// <summary>
        /// Get restaurant by ID (public endpoint)
        /// </summary>
        [HttpGet("{restaurantId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurant by ID", Description = "Retrieve specific restaurant details")]
        [SwaggerResponse(200, "Success", typeof(RestaurantDTO))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult> GetRestaurantById(int restaurantId)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);
            
            if (restaurant != null)
            {
                return ResponseHelper.Success(restaurant);
            }
            
            return ResponseHelper.NotFound("Restaurant", restaurantId);
        }

        /// <summary>
        /// Get restaurant menu (public endpoint)
        /// </summary>
        [HttpGet("{restaurantId}/menu")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurant menu", Description = "Retrieve menu items for specific restaurant")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ItemResponseDTO>))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult> GetMenu(int restaurantId, [FromQuery] string sortbyprice = "")
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return ResponseHelper.NotFound("Restaurant", restaurantId);
            }

            var menu = await _menuService.GetMenuAsync(restaurantId, sortbyprice);
            return ResponseHelper.Success(menu);
        }

        /// <summary>
        /// Add item to restaurant menu (admin or owner only)
        /// </summary>
        [HttpPost("{restaurantId}/items")]
        [Authorize(Policy = AuthorizationPolicies.CanManageRestaurant)]
        [SwaggerOperation(Summary = "Add menu item", Description = "Add new menu item to restaurant")]
        [SwaggerResponse(201, "Item added successfully", typeof(ItemDTO))]
        [SwaggerResponse(404, "Restaurant not found")]
        [SwaggerResponse(403, "Forbidden - admin or owner access required")]
        public async Task<ActionResult> AddItemToMenu(int restaurantId, [FromBody] ItemDTO itemDTO)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return ResponseHelper.NotFound("Restaurant", restaurantId);
            }

            var newItem = await _itemService.AddItemToMenuAsync(restaurantId, itemDTO);
            return ResponseHelper.Created(newItem);
        }

        /// <summary>
        /// Upload image file (authenticated, rate limited)
        /// </summary>
        [HttpPost("upload-image")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "Upload image file", Description = "Upload image for menu items")]
        [SwaggerResponse(200, "Image uploaded successfully")]
        [SwaggerResponse(400, "Invalid image file")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(413, "File too large")]
        [RateLimit(maxRequests: 20, timeWindowMinutes: 1)]
        public async Task<ActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return ResponseHelper.Error("No image file provided");
            }

            if (!FileHelper.IsValidImage(image))
            {
                return ResponseHelper.Error("Invalid image file. Allowed formats: JPEG, PNG, GIF, WebP. Max size: 5MB");
            }

            var imagePath = await _imageService.SaveImageAsync(image);
            var imageUrl = _imageService.GetImageUrl(imagePath);

            return ResponseHelper.Success(new { 
                imagePath, 
                imageUrl 
            }, "Image uploaded successfully");
        }

        /// <summary>
        /// Upload base64 encoded image (authenticated, rate limited)
        /// </summary>
        [HttpPost("upload-base64-image")]
        [Authorize(Policy = AuthorizationPolicies.Authenticated)]
        [SwaggerOperation(Summary = "Upload base64 image", Description = "Upload image from base64 string")]
        [SwaggerResponse(200, "Image uploaded successfully")]
        [SwaggerResponse(400, "Invalid image data")]
        [SwaggerResponse(401, "Unauthorized - JWT token required")]
        [SwaggerResponse(413, "File too large")]
        [RateLimit(maxRequests: 20, timeWindowMinutes: 1)]
        public async Task<ActionResult> UploadBase64Image([FromBody] ImageRequestDTO request)
        {
            if (string.IsNullOrEmpty(request?.Base64Image))
            {
                return ResponseHelper.Error("No image data provided");
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
                return ResponseHelper.Error("Invalid base64 image data");
            }

            // Sanitize filename
            var fileName = string.IsNullOrWhiteSpace(request.FileName) 
                ? "item" 
                : request.FileName.Replace("\\", "").Replace("/", "").Replace("..", "");

            var imagePath = await _imageService.SaveBase64ImageAsync(request.Base64Image, fileName);
            var imageUrl = _imageService.GetImageUrl(imagePath);

            return ResponseHelper.Success(new { 
                imagePath, 
                imageUrl 
            }, "Base64 image uploaded successfully");
        }

        /// <summary>
        /// Get all menu items (public endpoint)
        /// Phase B.4: Supports pagination via pageNumber and pageSize query parameters
        /// </summary>
        [HttpGet("items/all")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all menu items", Description = "Retrieve all menu items across all restaurants with pagination")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<ItemResponseDTO>))]
        [SwaggerResponse(404, "No items found")]
        public async Task<ActionResult> GetAllItems(
            [FromQuery] string itemName = "", 
            [FromQuery] string sortbyprice = "",
            [FromQuery(Name = "pageNumber")] string? pageNumberStr = null,
            [FromQuery(Name = "pageSize")] string? pageSizeStr = null)
        {
            // Extract and validate pagination parameters
            var paginationParams = PaginationHelper.ExtractFromQuery(pageNumberStr, pageSizeStr);
            
            var items = await _itemService.GetAllItemsAsync(itemName, sortbyprice);
            var itemList = items.ToList();

            if (!itemList.Any())
            {
                return ResponseHelper.NotFound("Items");
            }

            // Apply pagination manually (until service layer is updated)
            var totalCount = itemList.Count;
            var paginatedItems = itemList
                .Skip(paginationParams.GetOffset())
                .Take(paginationParams.PageSize)
                .ToList();

            var paginatedResponse = PaginatedResponse<ItemResponseDTO>.Create(
                paginatedItems, 
                paginationParams.PageNumber, 
                paginationParams.PageSize, 
                totalCount);

            return ResponseHelper.PaginatedStandard(paginatedResponse);
        }
    }
}
