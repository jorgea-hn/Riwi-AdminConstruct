using AdminConstruct.Web.Models;
using AdminConstruct.API.DTOs;
using AutoMapper;

namespace AdminConstruct.API.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}