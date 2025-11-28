using AdminConstruct.Web.Models;
using AdminConstruct.API.DTOs;
using AutoMapper;

namespace AdminConstruct.API.Mapping
{
    public class SaleDetailProfile : Profile
    {
        public SaleDetailProfile()
        {
            CreateMap<SaleDetail, SaleDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ReverseMap();
        }
    }
}