using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Mapping;

public class ImageUrlResolver : IValueResolver<Item, GetItems, string>
{
    private readonly IImageService _imageService;

    public ImageUrlResolver(IImageService imageService)
    {
        _imageService = imageService;
    }

    public string Resolve(Item source, GetItems destination, string destMember, ResolutionContext context)
    {
        return _imageService.GetImageUrl(source.ImageUrl);
    }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Restaurant mappings
        CreateMap<RestaurantDTO, Restaurant>();
        CreateMap<Restaurant, RestaurantDTO>();

        // User mappings
        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.Usercode, opt => opt.Ignore()) // Usercode is generated
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password is hashed separately
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Password, opt => opt.Ignore()); // Never expose password or hash

        // Item mappings
        CreateMap<ItemDTO, Item>()
            .ForMember(dest => dest.ItemID, opt => opt.Ignore())
            .ForMember(dest => dest.RestaurantID, opt => opt.Ignore())
            .ForMember(dest => dest.Restaurant, opt => opt.Ignore());

        CreateMap<Item, ItemDTO>();

        // GetItems mapping (for menu display)
        CreateMap<Item, GetItems>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<ImageUrlResolver>());

        // Order mappings
        CreateMap<OrderDTO, Order>()
            .ForMember(dest => dest.OrderID, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.UserID, opt => opt.Ignore())
            .ForMember(dest => dest.ItemPrice, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.MasterID, opt => opt.Ignore());

        // Order response mappings (no entity exposure)
        CreateMap<Order, OrderLineDTO>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ItemName))
            .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.ItemPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

        // Cart mappings
        CreateMap<Cart, CartItemDTO>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.ItemPrice * src.Quantity));

        // MasterOrder mappings
        CreateMap<MasterOrder, MasterOrderDTO>();
        CreateMap<MasterOrder, MasterOrderWithItemsDTO>()
            .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.Usercode))
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Orders));

        CreateMap<MasterOrderDTO, MasterOrder>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Restaurant, opt => opt.Ignore());
    }
}