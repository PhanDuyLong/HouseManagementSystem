using AutoMapper;
using HMS.Data.Models;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.HouseViewModels;
using System.Linq;

namespace HMS.Data.AutoMapperProfile
{
    public class MappingProfile : Profile {
        public MappingProfile()
        {
            CreateMap<Account, AccountViewModel>();

            CreateMap<HouseInfo, HouseInfoViewModel>();

            CreateMap<House, HouseHomeViewModel>()
                .ForMember(dest => dest.HouseInfo, opt => opt.MapFrom(src => src.HouseInfos.OfType<HouseInfo>().FirstOrDefault()));

            CreateMap<House, HouseDetailViewModel>()
                .ForMember(dest => dest.HouseInfo, opt => opt.MapFrom(src => src.HouseInfos.OfType<HouseInfo>().FirstOrDefault()));

            CreateMap<Room, RoomViewModel>();

            CreateMap<Service, ServiceViewModel>()
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.Name));
        }
    }
}
