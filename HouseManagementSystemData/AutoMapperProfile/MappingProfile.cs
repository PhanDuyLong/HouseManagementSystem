using AutoMapper;
using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.ViewModels;


namespace HouseManagementSystem.Data.AutoMapperProfile
{
    public class MappingProfile : Profile {
        public MappingProfile()
        {
            CreateMap<House, HouseViewModel>();

            CreateMap<HouseInfo, HouseInfoViewModel>();

            CreateMap<Account, AccountViewModel>();
        }
    }
}
