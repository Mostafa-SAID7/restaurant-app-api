using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// AutoMapper profile for external request/response DTOs
/// Phase B.1: Separates external API contracts from internal DTOs
/// </summary>
public class ExternalMappingProfile : Profile
{
    public ExternalMappingProfile()
    {
        // User mappings
        CreateMap<ExternalUserRequest, UserDTO>();
        CreateMap<User, ExternalUserResponse>()
            .ForMember(dest => dest.Usercode, opt => opt.MapFrom(src => src.Usercode));

        // Restaurant mappings
        CreateMap<ExternalRestaurantRequest, RestaurantDTO>();
        CreateMap<Restaurant, ExternalRestaurantResponse>();

        // Item mappings
        CreateMap<ExternalItemRequest, ItemDTO>();
        CreateMap<ItemResponseDTO, ExternalItemResponse>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // Will be set in controller if needed

        // Order mappings
        CreateMap<ExternalOrderLineRequest, OrderDTO>();
        CreateMap<ExternalCreateOrderRequest, CreateOrderRequestDTO>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderLineDTO, ExternalOrderLineResponse>();
        CreateMap<OrderResponseDTO, ExternalOrderResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        // Cart mappings
        CreateMap<ExternalCartItemRequest, AddCartItemRequestDTO>();
        CreateMap<CartItemDTO, ExternalCartItemResponse>();
        CreateMap<CartDTO, ExternalCartResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));

        // Image mappings
        CreateMap<ExternalImageUploadRequest, ImageRequestDTO>();
    }
}
