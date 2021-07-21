using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Contract;
using HMS.Data.ViewModels.Contract.Base;
using HMS.FirebaseNotification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IContractService : IBaseService<Contract>
    {
        ContractDetailViewModel GetById(int id);
        List<ContractDetailViewModel> GetByUserId(string userId);
        Task<ResultResponse> CreateContractAsync(string userId, CreateContractViewModel model);
        Task<ResultResponse> UpdateContractAsync(UpdateContractViewModel model);
        Task<ResultResponse> DeleteContractAsync(int contractId);
        List<ContractDetailViewModel> FilterByParamter(string userId, ContractParameters contractParameters);
        List<ContractDetailViewModel> GetByRoomId(int roomId);
        Task ScanContracts();
        string GetRoomDetail(int contractId);
        HouseDetailViewModel GetHouseByContractId(int contractId);
    }
    public partial class ContractService : BaseService<Contract>, IContractService
    {
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IServiceContractService _serviceContractService;
        private readonly IRoomService _roomService;
        private readonly IHouseService _houseService;
        private readonly IFirebaseNotificationService _firebaseNotificationService;
        public ContractService(DbContext dbContext, IContractRepository repository, IMapper mapper
            , IAccountService accountService, IServiceContractService serviceContractService, IRoomService roomService, IHouseService houseService, IFirebaseNotificationService firebaseNotificationService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _mapper = mapper;
            _accountService = accountService;
            _serviceContractService = serviceContractService;
            _roomService = roomService;
            _houseService = houseService;
            _firebaseNotificationService = firebaseNotificationService;
        }

        public async Task<ResultResponse> CreateContractAsync(string userId, CreateContractViewModel model)
        {
            var check = await _roomService.CheckInActiveRoomAsync(model.RoomId);
            if (!check.IsSuccess)
            {
                return check;
            }

            check = await CheckContractDetailAsync(model);
            if (!check.IsSuccess)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR02", new string[] { "Contract" }).Value + "\n" + check.Message,
                    IsSuccess = false
                };
            }

            var contract = _mapper.Map<Contract>(model);
            contract.OwnerUserId = userId;
            contract.Status = ContractConstants.CONTRACT_IS_ACTIVE;
            await CreateAsyn(contract);

            if (model.CreateServiceContracts != null && model.CreateServiceContracts.Count != 0)
            {
                var result = await _serviceContractService.CreateServiceContractsAsync(contract.RoomId.Value, contract.Id, model.CreateServiceContracts.ToList());
                if (!result.IsSuccess)
                {
                    return new ResultResponse
                    {
                        Message = result.Message,
                        IsSuccess = false
                    };
                }
            }

            await _roomService.UpdateRoomStatusAsync(model.RoomId, RoomConstants.ROOM_IS_RENTED);

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Contract" }).Value,
                IsSuccess = true,
            };
        }

        private async Task<ResultResponse> CheckContractDetailAsync(CreateContractViewModel model)
        {
            var room = _roomService.GetById(model.RoomId);
            if (room == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Room" }).Value,
                    IsSuccess = false,
                };
            }

            var tenantCheck = await _accountService.CheckValidUserAsync(model.TenantUserId);
            if (!tenantCheck.IsSuccess)
                return tenantCheck;

            return new ResultResponse
            {
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> DeleteContractAsync(int contractId)
        {
            var contractModel = GetById(contractId);
            if (contractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Contract" }).Value,
                    IsSuccess = false
                };
            }


            var contract = await GetAsyn(contractId);
            contract.IsDeleted = ContractConstants.CONTRACT_IS_DELETED;
            contract.Status = ContractConstants.CONTRACT_IS_INACTIVE;
            Update(contract);

            await _roomService.UpdateRoomStatusAsync(contractModel.RoomId, RoomConstants.ROOM_IS_NOT_RENTED);

            string roomDetail = "phòng " + contractModel.RoomName + "thuộc nhà " + contractModel.HouseName;

            var tenant = _accountService.GetByUserId(contract.TenantUserId);
            string message = string.Format(NotificationConstants.TENANT_CONTRACT_IS_DELETED, roomDetail);
            await SendContractNotificationAsync(tenant.Name, "all", message, contractId);

            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Contract" }).Value,
                IsSuccess = true
            };
        }

        public ContractDetailViewModel GetById(int id)
        {
            var contract = Get().Where(c => c.Id == id).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            if (contract != null)
            {
                var room = _roomService.GetById(contract.RoomId);
                var house = _houseService.GetById(room.HouseId);
                contract.RoomName = room.Name;
                contract.HouseName = house.HouseInfo.Name;
                contract.OwnerName = house.OwnerUser.Name;
                foreach(var serviceContract in contract.ServiceContracts)
                {
                    if(serviceContract.Clock != null && serviceContract.Clock.ClockValues != null && serviceContract.Clock.ClockValues.Count != 0)
                    {
                        serviceContract.Clock.ClockValues = serviceContract.Clock.ClockValues.Where(value => value.Status == ClockValueConstants.CLOCK_VALUE_IS_MILESTONE).ToList();
                    }
                }
            }
            return contract;
        }

        public List<ContractDetailViewModel> GetByUserId(string userId)
        {
            var user = _accountService.GetByUserId(userId);
            var contracts = new List<ContractDetailViewModel>();
            if (user != null)
            {
                if (user.Role.Equals(AccountConstants.ROLE_IS_OWNER))
                {
                    contracts = Get().Where(c => c.OwnerUserId == userId).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
                }
                else
                {
                    contracts = Get().Where(c => c.TenantUserId == userId).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
                }
                if(contracts != null && contracts.Count != 0)
                {
                    foreach (var contract in contracts)
                    {
                        var room = _roomService.GetById(contract.RoomId);
                        if(room == null)
                        {
                            contract.RoomName = null;
                        }
                        else
                        {
                            contract.RoomName = room.Name;
                            var house = _houseService.GetById(room.HouseId);
                            if(house != null)
                            {
                                contract.HouseName = house.HouseInfo.Name;
                                contract.OwnerName = house.OwnerUser.Name;
                            }
                            else
                            {
                                contract.HouseName = null;
                            }
                        }
                    }
                }
            }
            return contracts;
        }

        public async Task<ResultResponse> UpdateContractAsync(UpdateContractViewModel model)
        {
            int contractId = model.Id;
            var contractModel = GetById(contractId);
            if (contractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Contract" }).Value,
                    IsSuccess = false,
                };
            }
            var contract = await GetAsyn(model.Id);

            /*var nowContracts = GetByRoomId(contract.RoomId.Value).Where(c => c.Status == ContractConstants.CONTRACT_IS_ACTIVE).FirstOrDefault();
            if(nowContracts != null)
            {
                return new ResultResponse
                {
                    Message = "Room has an active contract",
                    IsSuccess = false
                };
            }*/


            if (model.TenantUserId != null)
            {
                contract.TenantUserId = model.TenantUserId;
            }
            if (model.StartDate != null)
            {
                contract.StartDate = model.StartDate;
            }
            if (model.EndDate != null)
            {
                contract.EndDate = model.EndDate;
            }
            if(model.RoomPrice != null)
            {
                contract.RoomPrice = model.RoomPrice;
            }
            if(model.Note != null)
            {
                contract.Note = model.Note;
            }
            contract.Status = ContractConstants.CONTRACT_IS_ACTIVE;
            contract.IsDeleted = ContractConstants.CONTRACT_IS_NOT_DELETED;
            contract.IsSent = ContractConstants.CONTRACT_IS_NOT_SENT;
            Update(contract);

            var tenant = _accountService.GetByUserId(contract.TenantUserId);

            string roomDetail = "phòng " + contractModel.RoomName + "thuộc nhà " + contractModel.HouseName;
            string message = string.Format(NotificationConstants.TENANT_CONTRACT_IS_UPDATED, roomDetail);
            await SendContractNotificationAsync(tenant.Name, "all", message, contractId);

            var check = await _serviceContractService.UpdateServiceContractsAsync(contractModel.RoomId, contractId, model.UpdateServiceContracts.ToList());
            if (!check.IsSuccess)
                return check;

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Contract" }).Value,
                IsSuccess = true
            };

        }

        public List<ContractDetailViewModel> GetByRoomId(int roomId)
        {
            var contracts = Get().Where(c => c.RoomId == roomId).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return contracts;
        }

        public List<ContractDetailViewModel> FilterByParamter(string userId, ContractParameters contractParameters)
        {
            List<ContractDetailViewModel> contracts;
            var roomId = contractParameters.RoomId;
            if (roomId != null)
            {
                contracts = GetByRoomId(roomId.Value);
            }
            else
            {
                contracts = GetByUserId(userId);
            }
            var status = contractParameters.Status;
            if (status != null)
            {
                contracts = contracts.Where(c => c.Status == contractParameters.Status).ToList();
            }
            if (contractParameters.IsDeleted != null)
            {
                contracts = contracts.Where(c => c.IsDeleted == contractParameters.IsDeleted).ToList();
            }
            return contracts;
        }

        public List<ContractDetailViewModel> GetActiveContract()
        {
            var contracts = Get().Where(c => c.Status == ContractConstants.CONTRACT_IS_ACTIVE && c.IsDeleted == ContractConstants.CONTRACT_IS_NOT_DELETED).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return contracts;
        }

        public async Task<ResultResponse> SendContractNotificationAsync(string title, string userId, string message, int contractId)
        {
            MobileNotification firebaseNotification = new MobileNotification
            {
                UserId = userId,
                Title = title,
                Body = message,
                Data = new Dictionary<string, string>
                {
                    { "contractId", contractId.ToString()},
                }
            };

            await _firebaseNotificationService.PushNotificationAsync(firebaseNotification);
            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "ContractId" }).Value,
                IsSuccess = true
            };
        }

        public string GetRoomDetail(int contractId)
        {
            var contract = GetById(contractId);
            return "phòng " + contract.RoomName + "thuộc nhà " + contract.HouseName;
        }

        public async Task<ResultResponse> SetContractIsSent(int contractId)
        {
            var contract = await GetAsyn(contractId);
            contract.IsSent = true;
            Update(contract);
            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Contract" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> SetContractInactive(int contractId)
        {
            var contractModel = GetById(contractId);
            if (contractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Contract" }).Value,
                    IsSuccess = false,
                };
            }

            var contract = await GetAsyn(contractId);
            contract.Status = ContractConstants.CONTRACT_IS_INACTIVE;
            Update(contract);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Contract" }).Value,
                IsSuccess = true
            };
        }


        public async Task ScanContracts()
        {
            var contracts = GetActiveContract();
            foreach (var contract in contracts)
            {
                if (contract.EndDate < DateTime.Now)
                {
                    await SetContractInactive(contract.Id);

                    var owner = _accountService.GetByUserId(contract.OwnerUserId);
                    var tenant = _accountService.GetByUserId(contract.TenantUserId);
                    

                    var ownerUserId = "all";
                    string roomDetail = "phòng " + contract.RoomName + "thuộc nhà " + contract.HouseName;
                    string ownerMessage = string.Format(NotificationConstants.ONWER_CONTRACT_HAS_EXPIRED, roomDetail);
                    await SendContractNotificationAsync(owner.Name, ownerUserId, ownerMessage, contract.Id);
                    var tenantUserId = "all";
                    string tenantMessage = string.Format(NotificationConstants.TENANT_CONTRACT_HAS_EXPIRED, roomDetail);
                    await SendContractNotificationAsync(tenant.Name ,tenantUserId, tenantMessage, contract.Id);
                    await SetContractIsSent(contract.Id);
                    break;
                }

                if ((contract.EndDate.Value - DateTime.Today).TotalDays < 7)
                {
                    var owner = _accountService.GetByUserId(contract.OwnerUserId);
                    var tenant = _accountService.GetByUserId(contract.TenantUserId);

                    var ownerUserId = "all";
                    string roomDetail = GetRoomDetail(contract.Id);
                    string ownerMessage = string.Format(NotificationConstants.ONWER_CONTRACT_WILL_EXPIRE_IN_1_WEEK, roomDetail);
                    await SendContractNotificationAsync(owner.Name, ownerUserId, ownerMessage, contract.Id);
                    var tenantUserId = "all";
                    string tenantMessage = string.Format(NotificationConstants.TENANT_CONTRACT_WILL_EXPIRE_IN_1_WEEK, roomDetail);
                    await SendContractNotificationAsync(tenant.Name, tenantUserId, tenantMessage, contract.Id);
                    await SetContractIsSent(contract.Id);
                    break;
                }
            }
        }

        public HouseDetailViewModel GetHouseByContractId(int contractId)
        {
            var room = _roomService.GetById(contractId);
            var house = _houseService.GetById(room.HouseId);
            return house;
        }
    }
}
