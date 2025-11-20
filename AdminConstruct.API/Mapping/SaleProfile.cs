using AdminConstruct.Web.Models;
using AdminConstruct.API.DTOs;
using AutoMapper;

namespace AdminConstruct.API.Mapping
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            CreateMap<SaleDto, Sale>();
        }
    }
}