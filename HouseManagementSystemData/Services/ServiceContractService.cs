using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HMS.Data.Services
{
    public partial interface IServiceContractService : IBaseService<ServiceContract>
    {
        List<ServiceContractDetailViewModel> GetByContractId(int contractId);
        ServiceContractDetailViewModel GetById(int id);
        Task<ResultResponse> UpdateServiceContractAsync(UpdateServiceContractViewModel model);
        Task<ResultResponse> DeleteServiceContractAsync(int serviceContractId);
    }
    public partial class ServiceContractService : BaseService<ServiceContract>, IServiceContractService
    {
        private readonly IMapper _mapper;
        public ServiceContractService(DbContext dbContext, IServiceContractRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResultResponse> DeleteServiceContractAsync(int serviceContractId)
        {
            var serviceContractModel = GetById(serviceContractId);
            if (serviceContractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "ServiceContract" }).Value,
                    IsSuccess = false,
                };
            }
            var serviceContract = await GetAsyn(serviceContractId);
            serviceContract.Status = ServiceContractConstants.SERVICE_CONTRACT_IS_INACTIVE;
            Update(serviceContract);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "ServiceContract" }).Value,
                IsSuccess = true
            };
        }

        public List<ServiceContractDetailViewModel> GetByContractId(int contractId)
        {
            List<ServiceContractDetailViewModel> contract;
            contract = Get().Where(sc => sc.Id == contractId && sc.Status == ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE).ProjectTo<ServiceContractDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return contract;
        }

        public ServiceContractDetailViewModel GetById(int id)
        {
            var serviceContract = Get().Where(sc => sc.Id == id && sc.Status == ServiceContractConstants.SERVICE_CONTRACT_IS_ACTIVE).ProjectTo<ServiceContractDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return serviceContract;
        }

        public async Task<ResultResponse> UpdateServiceContractAsync(UpdateServiceContractViewModel model)
        {
            var serviceContractId = model.Id;
            var serviceContractModel = GetById(serviceContractId);
            if (serviceContractModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "ServiceContract" }).Value,
                    IsSuccess = false
                };
            }
            var serviceContract = await GetAsyn(model.Id);
            if (model.UnitPrice!=null)
            {
                serviceContract.UnitPrice = model.UnitPrice;
            }
            Update(serviceContract);
            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "ServiceContract" }).Value,
                IsSuccess = true
            };
        }
    }

    
}

