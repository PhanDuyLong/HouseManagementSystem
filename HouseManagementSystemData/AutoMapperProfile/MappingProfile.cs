using AutoMapper;
using HMS.Data.Models;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.HouseViewModels;
using HMS.Data.ViewModels.RoomViewModels;
using System.Linq;

namespace HMS.Data.AutoMapperProfile
{
    public class MappingProfile : Profile {
        public MappingProfile()
        {
            CreateMap<Account, AccountBaseViewModel>();

            CreateMap<HouseInfo, HouseInfoViewModel>();

            CreateMap<House, HouseBaseViewModel>()
                .ForMember(dest => dest.HouseInfo, opt => opt.MapFrom(src => src.HouseInfos.OfType<HouseInfo>().FirstOrDefault()));

            CreateMap<House, HouseDetailViewModel>()
                .ForMember(dest => dest.HouseInfo, opt => opt.MapFrom(src => src.HouseInfos.OfType<HouseInfo>().FirstOrDefault()));

            CreateMap<Room, RoomShowViewModel>()
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contracts.OfType<Contract>().FirstOrDefault()));

            CreateMap<Room, RoomDetailViewModel>()
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contracts.OfType<Contract>().FirstOrDefault()));

            CreateMap<Service, ServiceViewModel>()
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.Name));

            CreateMap<Contract, ContractBaseViewModel>();

        }
    }
}
