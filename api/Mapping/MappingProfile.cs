using AutoMapper;
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

        // Cart mappings
        CreateMap<SetCart, CartDTO>()
            .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.item.ItemID))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.item.ItemName))
            .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.item.ItemPrice))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.item.ItemPrice * src.Quantity))
            .ForMember(dest => dest.UserID, opt => opt.Ignore());

        CreateMap<Cart, CartDTO>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.ItemPrice * src.Quantity));

        // MasterOrder mappings
        CreateMap<MasterOrder, MasterOrderDTO>();
        CreateMap<MasterOrderDTO, MasterOrder>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Restaurant, opt => opt.Ignore());
    }
}