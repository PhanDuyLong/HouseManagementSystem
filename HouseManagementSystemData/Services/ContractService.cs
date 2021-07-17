using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.Contract;
using HMS.Data.ViewModels.Contract.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IContractService : IBaseService<Contract>
    {
        ContractDetailViewModel GetById(int id);
        Task<ResultResponse> CreateContractAsync(string userId, CreateContractViewModel model);
        Task<ResultResponse> UpdateContractAsync(UpdateContractViewModel model);
        Task<ResultResponse> DeleteContractAsync(int contractId);
        List<ContractDetailViewModel> GetByUserId(string userId);
    }
    public partial class ContractService : BaseService<Contract>, IContractService
    {
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IServiceContractService _serviceContractService;
        private readonly IRoomService _roomService;
        private readonly IHouseService _houseService;
        public ContractService(DbContext dbContext, IContractRepository repository, IMapper mapper
            , IAccountService accountService, IServiceContractService serviceContractService, IRoomService roomService, IHouseService houseService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _mapper = mapper;
            _accountService = accountService;
            _serviceContractService = serviceContractService;
            _roomService = roomService;
            _houseService = houseService;
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
            contract.Status = ContractConstants.CONTRACT_IS_INACTIVE;
            Update(contract);

            await _roomService.UpdateRoomStatusAsync(contractModel.RoomId, RoomConstants.ROOM_IS_NOT_RENTED);

            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Contract" }).Value,
                IsSuccess = true
            };
        }

        public ContractDetailViewModel GetById(int id)
        {
            var contract = Get().Where(c => c.Id == id && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            if(contract != null)
            {
                var room = _roomService.GetById(contract.RoomId);
                var house = _houseService.GetById(room.HouseId);
                contract.RoomName = room.Name;
                contract.HouseName = house.HouseInfo.Name;
                contract.OwnerName = house.OwnerUser.Name;
            }
            return contract;
        }

        public List<ContractDetailViewModel> GetByUserId(string userId)
        {
            var user = _accountService.GetByUserId(userId);
            var contract = new List<ContractDetailViewModel>();
            if (user.Role.Equals(AccountConstants.ROLE_IS_OWNER))
            {
                contract = Get().Where(c => c.OwnerUserId == userId && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            else
            {
                contract = Get().Where(c => c.TenantUserId == userId && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            return contract;
        }

        public async Task<ResultResponse> UpdateContractAsync(UpdateContractViewModel model)
        {
            int contractId = (int)model.Id;
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
            Update(contract);
            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Contract" }).Value,
                IsSuccess = true
            };

        }
    }
}
