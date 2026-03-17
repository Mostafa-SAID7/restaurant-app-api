using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FakeRestuarantAPI.Controllers
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
    public async Task<ActionResult> getrestaurants([FromQuery] string category="", string address=null, string name=null)
    {
        try
        {
            var restaurants = await _restaurantService.GetRestaurantsAsync(category, address, name);
            
            if (restaurants.Any())
            {
                return Ok(restaurants);
            }
            
            return StatusCode(404, "No Restaurants Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting restaurants");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new restaurant", Description = "Add a new restaurant to the system")]
    [SwaggerResponse(201, "Restaurant created successfully", typeof(Restaurant))]
    [SwaggerResponse(409, "Restaurant already exists")]
    public async Task<ActionResult> addrestaurant(RestaurantDTO restaurantDTO)
    {
        try
        {
            var restaurantExists = await _restaurantService.RestaurantExistsAsync(restaurantDTO.RestaurantName);
            if (restaurantExists)
            {
                return Conflict(new { message = "Restaurant Already Exists" });
            }

            var newRestaurant = await _restaurantService.CreateRestaurantAsync(restaurantDTO);
            return StatusCode(201, newRestaurant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating restaurant");
            return StatusCode(500, "Internal server error");
        }
    }

     [HttpGet("{Restaurant_id}")]
     [SwaggerOperation(Summary = "Get restaurant by ID", Description = "Retrieve a specific restaurant by its ID")]
     [SwaggerResponse(200, "Success", typeof(Restaurant))]
     [SwaggerResponse(404, "Restaurant not found")]
     public async Task<ActionResult> getrestaurantbyid([FromRoute] int Restaurant_id)
     {
        try
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(Restaurant_id);
            
            if (restaurant != null)
            {
                return Ok(restaurant);
            }
            
            return StatusCode(404, $"No Restaurant Exists with {Restaurant_id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting restaurant by id");
            return StatusCode(500, "Internal server error");
        }
     }



        [HttpGet("{Restaurant_id}/menu")]
        [SwaggerOperation(Summary = "Get restaurant menu", Description = "Retrieve menu items for a specific restaurant with optional price sorting")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<GetItems>))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult> getmenu(int Restaurant_id,[FromQuery] string sortbyprice = "")
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantByIdAsync(Restaurant_id);
                if (restaurant == null)
                {
                    return StatusCode(404, $"No Restaurant Exists with id:{Restaurant_id}");
                }

                var menu = await _restaurantService.GetMenuAsync(Restaurant_id, sortbyprice);
                return Ok(menu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{Restaurant_id}/additem")]
        [SwaggerOperation(Summary = "Add item to menu", Description = "Add a new menu item to a specific restaurant")]
        [SwaggerResponse(201, "Item added successfully", typeof(Item))]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<ActionResult> setmenu(int Restaurant_id, [FromBody] ItemDTO itemDTO)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantByIdAsync(Restaurant_id);
                if (restaurant == null)
                {
                    return StatusCode(404, $"No Restaurant Exists with id:{Restaurant_id}");
                }

                var newItem = await _restaurantService.AddItemToMenuAsync(Restaurant_id, itemDTO);
                return StatusCode(201, newItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to menu");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("upload-image")]
        [SwaggerOperation(Summary = "Upload image file", Description = "Upload an image file for menu items")]
        [SwaggerResponse(200, "Image uploaded successfully")]
        [SwaggerResponse(400, "Invalid image file")]
        public async Task<ActionResult> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest("No image file provided");
                }

                var imagePath = await _imageService.SaveImageAsync(image);
                var imageUrl = _imageService.GetImageUrl(imagePath);

                return Ok(new { imagePath, imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, "Error uploading image");
            }
        }

        [HttpPost("upload-base64-image")]
        [SwaggerOperation(Summary = "Upload base64 image", Description = "Upload an image from base64 string")]
        [SwaggerResponse(200, "Image uploaded successfully")]
        [SwaggerResponse(400, "Invalid image data")]
        public async Task<ActionResult> UploadBase64Image([FromBody] ImageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Base64Image))
                {
                    return BadRequest("No image data provided");
                }

                var imagePath = await _imageService.SaveBase64ImageAsync(request.Base64Image, request.FileName ?? "item");
                var imageUrl = _imageService.GetImageUrl(imagePath);

                return Ok(new { imagePath, imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading base64 image");
                return StatusCode(500, "Error uploading image");
            }
        }

        [HttpGet("items")]
        [SwaggerOperation(Summary = "Get all items", Description = "Retrieve all menu items across all restaurants with optional filtering")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<GetItems>))]
        [SwaggerResponse(404, "No items found")]
        public async Task<ActionResult> getitems([FromQuery] string ItemName = "", string sortbyprice = "")
        {
            try
            {
                var items = await _restaurantService.GetAllItemsAsync(ItemName, sortbyprice);

                if (items.Any())
                {
                    return Ok(items);
                }

                return StatusCode(404, "No items Found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
