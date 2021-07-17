using AutoMapper;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Requests;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Account;
using HMS.Data.ViewModels.Bill;
using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Contract;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.HouseViewModels;
using HMS.Data.ViewModels.Room;
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
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AccountConstants.ACCOUNT_IS_ACTIVE));

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
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contracts.OfType<Contract>().Where(c => c.Status == ContractConstants.CONTRACT_IS_ACTIVE).FirstOrDefault()));

            CreateMap<CreateRoomViewModel, Room>();

            CreateMap<Service, ServiceViewModel>()
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.Name));

            CreateMap<CreateServiceViewModel, Service>();

            CreateMap<Contract, ContractBaseViewModel>();

            CreateMap<Clock, ClockDetailViewModel>()
                .ForMember(dest => dest.ClockCategory, opt => opt.MapFrom(src => src.ClockCategory.Name));
            CreateMap<CreateClockViewModel, Clock>();

            CreateMap<Contract, ContractDetailViewModel>();
            CreateMap<CreateContractViewModel, Contract>();

            CreateMap<ServiceContract, ServiceContractDetailViewModel>();
            CreateMap<CreateServiceContractViewModel, ServiceContract>();

            CreateMap<ClockValue, ClockValueDetailViewModel>();
            CreateMap<CreateClockValueViewModel, ClockValue>();

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
