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
        public ContractService(DbContext dbContext, IContractRepository repository, IMapper mapper, IAccountService accountService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _repository = repository;
            _mapper = mapper;
            _accountService = accountService;
        }

        public async Task<ResultResponse> CreateContractAsync(string userId, CreateContractViewModel model)
        {
            var contract = _mapper.Map<Contract>(model);
            contract.OwnerUserId = userId;
            contract.Status = ContractConstants.CONTRACT_IS_INACTIVE;
            await CreateAsyn(contract);
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Contract" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> DeleteContractAsync(int contractId)
        {
            var contractModel = GetById(contractId);
            if (contractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Contract" }),
                    IsSuccess = false
                };

            }
            var contractService = await GetAsyn(contractId);
            contractService.Status = ContractServiceConstants.CONTRACT_SERVICE_IS_INACTIVE;
            Update(contractService);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Contract" }).Value,
                IsSuccess = true
            };

        }
       

        public ContractDetailViewModel GetById(int id)
        {
            var contract = Get().Where(c => c.Id == id && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
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
            if (model.TenantUserId!=null) {
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
