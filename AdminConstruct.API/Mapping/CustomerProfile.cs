using AdminConstruct.Web.Models;
using AdminConstruct.API.DTOs;
using AutoMapper;

namespace AdminConstruct.API.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
        }
    }
}
