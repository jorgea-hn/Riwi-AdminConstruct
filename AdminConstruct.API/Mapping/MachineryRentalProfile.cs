using AutoMapper;
using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Models;

namespace AdminConstruct.API.Mapping
{
    public class MachineryRentalProfile : Profile
    {
        public MachineryRentalProfile()
        {
            CreateMap<MachineryRental, MachineryRentalDto>()
                .ForMember(dest => dest.MachineryName, opt => opt.MapFrom(src => src.Machinery.Name))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name)); // Asumiendo que Customer tiene Name

            CreateMap<CreateMachineryRentalDto, MachineryRental>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}
