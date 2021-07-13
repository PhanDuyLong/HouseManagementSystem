using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Requests;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Account;
using HMS.Data.ViewModels.Bill;
using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.HouseViewModels;
using HMS.Data.ViewModels.RoomViewModels;
using HMS.Data.ViewModels.ServiceContract;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HMS.Data.AutoMapperProfile
{
    public class MappingProfile : Profile {
        public MappingProfile()
        {
            CreateMap<Account, AccountDetailViewModel>();

            CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true));

            CreateMap<Account, AccountTenantViewModel>();

            CreateMap<HouseInfo, HouseInfoViewModel>();
            CreateMap<UpdateHouseInfoViewModel, HouseInfo>();
            CreateMap<CreateHouseInfoViewModel, HouseInfo>();

            CreateMap<House, HouseBaseViewModel>()
                .ForMember(dest => dest.HouseInfo, opt => opt.MapFrom(src => src.HouseInfos.OfType<HouseInfo>().FirstOrDefault()));
            CreateMap<House, HouseDetailViewModel>()
                .ForMember(dest => dest.HouseInfo, opt => opt.MapFrom(src => src.HouseInfos.OfType<HouseInfo>().FirstOrDefault()));
            CreateMap<CreateHouseViewModel, House>()
                .ForMember(dest => dest.HouseInfos, opt => opt.MapFrom(src => new List<CreateHouseInfoViewModel>{src.HouseInfos}));

            CreateMap<Room, RoomBaseViewModel>();

            CreateMap<Room, RoomShowViewModel>()
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contracts.OfType<Contract>().FirstOrDefault()));

            CreateMap<Room, RoomDetailViewModel>()
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contracts.OfType<Contract>().FirstOrDefault()));

            CreateMap<Service, ServiceViewModel>()
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.Name));
            CreateMap<CreateServiceViewModel, Service>();


            CreateMap<Contract, ContractBaseViewModel>();

            CreateMap<Clock, ClockDetailViewModel>()
                .ForMember(dest => dest.ClockCategory, opt => opt.MapFrom(src => src.ClockCategory.Name));

            CreateMap<Contract, ContractDetailViewModel>();

            CreateMap<ServiceContract, ServiceContractDetailViewModel>();

            CreateMap<ClockValue, ClockValueDetailViewModel>();

            CreateMap<Bill, BillDetailViewModel>();
            CreateMap<Bill, ShowBillViewModel>();

            CreateMap<BillItem, BillItemDetailViewModel>();
            CreateMap<BillItem, ShowBillItemViewModel>();

            CreateMap<Payment, PaymentDetailViewModel>();

            CreateMap<CreateBillViewModel, Bill>();

            CreateMap<UpdateBillViewModel, Bill>();
        }
    }
}
