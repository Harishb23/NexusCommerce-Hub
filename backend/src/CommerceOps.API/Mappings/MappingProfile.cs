using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Models;

namespace CommerceOps.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDto>();

        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => false));

        CreateMap<SyncLog, SyncLogDto>();

        CreateMap<IntegrationHealth, IntegrationHealthDto>();
    }
}
