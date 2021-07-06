using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels.Contract;
using HMS.Data.ViewModels.Contract.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IContractService : IBaseService<Contract>
    {
        ContractDetailViewModel GetByID(int id);
        Task<ContractDetailViewModel> CreateContract(CreateContractViewModel model);
        ContractDetailViewModel UpdateContract(Contract contract, UpdateContractViewModel model);
        string DeleteContract(Contract contract);
        List<ContractDetailViewModel> GetByUsername(string username);
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

        public Task<ContractDetailViewModel> CreateContract(CreateContractViewModel model)
        {
            throw new System.NotImplementedException();
        }

        public string DeleteContract(Contract contract)
        {
            throw new System.NotImplementedException();
        }

        public ContractDetailViewModel GetByID(int id)
        {
            var contract = Get().Where(c => c.Id == id && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return contract;
        }

        public List<ContractDetailViewModel> GetByUsername(string username)
        {
            var user = _accountService.GetByUsername(username);
            var contract = new List<ContractDetailViewModel>();
            if (user.Role.Equals(AccountConstants.ROLE_IS_OWNER))
            {
                contract = Get().Where(c => c.OwnerUsername == username && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            else
            {
                contract = Get().Where(c => c.TenantUsername == username && c.Status == ContractConstants.CONTRACT_IS_ACTIVE).ProjectTo<ContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            }
            return contract;
        }

        public ContractDetailViewModel UpdateContract(Contract contract, UpdateContractViewModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
