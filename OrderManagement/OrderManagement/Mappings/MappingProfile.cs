using AutoMapper;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.DiscountType ?? "None"));

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>();

            // ✅ Product mapping - TaxStatus is computed automatically
            CreateMap<Product, ProductDto>();
        }
    }
}