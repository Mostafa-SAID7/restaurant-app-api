using AutoMapper;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for Application-layer DTO mappings.
/// All entity ↔ DTO conversions happen here, centralized.
/// No manual DTO construction in handlers.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Restaurant mappings
        CreateMap<Restaurant, RestaurantDto>().ReverseMap();
        CreateMap<CreateRestaurantDto, Restaurant>();

        // Item mappings
        CreateMap<Item, ItemDto>().ReverseMap();
        CreateMap<Item, ItemResponseDto>();
        CreateMap<CreateItemDto, Item>();

        // User mappings
        CreateMap<User, UserDto>();

        // Cart mappings
        CreateMap<Cart, CartItemDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.ItemPrice * src.Quantity));
        CreateMap<AddCartItemDto, Cart>()
            .ForMember(dest => dest.CartID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Item, opt => opt.Ignore());

        // Order mappings
        CreateMap<Order, OrderLineDto>();
        CreateMap<OrderLineInputDto, Order>()
            .ForMember(dest => dest.OrderID, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.UserID, opt => opt.Ignore())
            .ForMember(dest => dest.Item, opt => opt.Ignore())
            .ForMember(dest => dest.ItemPrice, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.MasterID, opt => opt.Ignore())
            .ForMember(dest => dest.MasterOrder, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // MasterOrder mappings
        CreateMap<MasterOrder, MasterOrderDto>();
        CreateMap<MasterOrder, MasterOrderWithItemsDto>()
            .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.Usercode))
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Orders));
    }
}
