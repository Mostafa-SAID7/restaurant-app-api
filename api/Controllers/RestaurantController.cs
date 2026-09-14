using RestaurantAPI.DTOs;
using RestaurantAPI.Filters;
using RestaurantAPI.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace RestaurantAPI.Controllers
{
    [Route("api/Restaurant")]
    [ApiController]
    [SwaggerTag("Restaurant management endpoints")]
    public class RestaurantController : ControllerBase
    {
        private readonly ILogger<RestaurantController> _logger;
        private readonly IRestaurantService _restaurantService;
        private readonly IImageService _imageService;

        public RestaurantController(ILogger<RestaurantController> logger, IRestaurantService restaurantService, IImageService imageService)
        {
            _logger = logger;
            _restaurantService = restaurantService;
            _imageService = imageService;
        }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all restaurants", Description = "Retrieve all restaurants with optional filtering by category, address, and name")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<Restaurant>))]
    [SwaggerResponse(404, "No restaurants found")]
    public async Task<ActionResult> getrestaurants([FromQuery] string category="", string? address=null, string? name=null)
    {
        var restaurants = await _restaurantService.GetRestaurantsAsync(category, address, name);
        
        if (restaurants.Any())
        {
            return ResponseHelper.Success(restaurants);
        }
        
        return ResponseHelper.NotFound("Restaurants");
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new restaurant", Description = "Add a new restaurant to the system")]
    [SwaggerResponse(201, "Restaurant created successfully", typeof(Restaurant))]
    [SwaggerResponse(409, "Restaurant already exists")]
    public async Task<ActionResult> addrestaurant(RestaurantDTO restaurantDTO)
    {
        var restaurantExists = await _restaurantService.RestaurantExistsAsync(restaurantDTO.RestaurantName);
        if (restaurantExists)
        {
            return ResponseHelper.Error("Restaurant already exists", 409);
        }

        var newRestaurant = await _restaurantService.CreateRestaurantAsync(restaurantDTO);
        return ResponseHelper.Created(newRestaurant);
    }

     [HttpGet("{Restaurant_id}")]
     [SwaggerOperation(Summary = "Get restaurant by ID", Description = "Retrieve a specific restaurant by its ID")]
     [SwaggerResponse(200, "Success", typeof(Restaurant))]
     [SwaggerResponse(404, "Restaurant not found")]
     public async Task<ActionResult> getrestaurantbyid([FromRoute] int Restaurant_id)
     {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(Restaurant_id);
        
        if (restaurant != null)
        {
            return ResponseHelper.Success(restaurant);
        }
        
        return ResponseHelper.NotFound("Restaurant", Restaurant_id);
     }



        [HttpGet("{Restaurant_id}/menu")]
        [SwaggerOperation(Summary = "Get restaurant menu", Description = "Retrieve menu items for a specific restaurant with optional price sorting")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<GetItems>))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult> getmenu(int Restaurant_id,[FromQuery] string sortbyprice = "")
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(Restaurant_id);
            if (restaurant == null)
            {
                return ResponseHelper.NotFound("Restaurant", Restaurant_id);
            }

            var menu = await _restaurantService.GetMenuAsync(Restaurant_id, sortbyprice);
            return ResponseHelper.Success(menu);
        }

        [HttpPost("{Restaurant_id}/additem")]
        [SwaggerOperation(Summary = "Add item to menu", Description = "Add a new menu item to a specific restaurant")]
        [SwaggerResponse(201, "Item added successfully", typeof(Item))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult> setmenu(int Restaurant_id, [FromBody] ItemDTO itemDTO)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(Restaurant_id);
            if (restaurant == null)
            {
                return ResponseHelper.NotFound("Restaurant", Restaurant_id);
            }

            var newItem = await _restaurantService.AddItemToMenuAsync(Restaurant_id, itemDTO);
            return ResponseHelper.Created(newItem);
        }

        [HttpPost("upload-image")]
        [SwaggerOperation(Summary = "Upload image file", Description = "Upload an image file for menu items (requires authentication)")]
        [SwaggerResponse(200, "Image uploaded successfully")]
        [SwaggerResponse(400, "Invalid image file")]
        [SwaggerResponse(401, "Unauthorized - authentication required")]
        [SwaggerResponse(413, "File too large")]
        [RequireApiKey]
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

        [HttpPost("upload-base64-image")]
        [SwaggerOperation(Summary = "Upload base64 image", Description = "Upload an image from base64 string (requires authentication)")]
        [SwaggerResponse(200, "Image uploaded successfully")]
        [SwaggerResponse(400, "Invalid image data")]
        [SwaggerResponse(401, "Unauthorized - authentication required")]
        [SwaggerResponse(413, "File too large")]
        [RequireApiKey]
        [RateLimit(maxRequests: 20, timeWindowMinutes: 1)]
        public async Task<ActionResult> UploadBase64Image([FromBody] ImageRequest request)
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

        [HttpGet("items")]
        [SwaggerOperation(Summary = "Get all items", Description = "Retrieve all menu items across all restaurants with optional filtering")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<GetItems>))]
        [SwaggerResponse(404, "No items found")]
        public async Task<ActionResult> getitems([FromQuery] string ItemName = "", string sortbyprice = "")
        {
            var items = await _restaurantService.GetAllItemsAsync(ItemName, sortbyprice);

            if (items.Any())
            {
                return ResponseHelper.Success(items);
            }

            return ResponseHelper.NotFound("Items");
        }
    }
}
