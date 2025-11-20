using AdminConstruct.Web.Models;
using AdminConstruct.API.DTOs;
using AutoMapper;

namespace AdminConstruct.API.Mapping
{
    public class SaleDetailProfile : Profile
    {
        public SaleDetailProfile()
        {
            CreateMap<SaleDetail, SaleDetailDto>().ReverseMap();
        }
    }
}