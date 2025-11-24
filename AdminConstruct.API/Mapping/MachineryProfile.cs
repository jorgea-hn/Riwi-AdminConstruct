using AutoMapper;
using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Models;

namespace AdminConstruct.API.Mapping
{
    public class MachineryProfile : Profile
    {
        public MachineryProfile()
        {
            CreateMap<Machinery, MachineryDto>();
            CreateMap<CreateMachineryDto, Machinery>();
        }
    }
}
