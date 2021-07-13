using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HMS.Data.Services
{
    public partial interface IServiceContractService : IBaseService<ServiceContract>
    {
        List<ServiceContractDetailViewModel> GetByContractId(int contractId);
        ServiceContractDetailViewModel GetByID(int id);
        ServiceContractDetailViewModel UpdateServiceContract(ServiceContract serviceContract, UpdateServiceContractViewModel model);
        string DeleteServiceContract(ServiceContract serviceContract);
    }
    public partial class ServiceContractService : BaseService<ServiceContract>, IServiceContractService
    {
        private readonly IMapper _mapper;
        public ServiceContractService(DbContext dbContext, IServiceContractRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public string DeleteServiceContract(ServiceContract serviceContract)
        {
            throw new System.NotImplementedException();
        }

        public List<ServiceContractDetailViewModel> GetByContractId(int contractId)
        {
            throw new System.NotImplementedException();
        }

        public ServiceContractDetailViewModel GetByID(int id)
        {
            throw new System.NotImplementedException();
        }

        public ServiceContractDetailViewModel UpdateServiceContract(ServiceContract serviceContract, UpdateServiceContractViewModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
