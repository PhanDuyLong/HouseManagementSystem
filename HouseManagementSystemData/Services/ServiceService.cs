using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IServiceService : IBaseService<Service>
    {
        List<ServiceViewModel> GetByHouseId(string houseId);
        ServiceViewModel GetByID(string id);
        Task<ServiceViewModel> CreateService(CreateServiceViewModel model);
        ServiceViewModel UpdateService(Service service, UpdateServiceViewModel model);
        string DeleteService(Service service);
    }
    public partial class ServiceService : BaseService<Service>, IServiceService
    {
        private readonly IMapper _mapper;
        public ServiceService(DbContext dbContext, IServiceRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public Task<ServiceViewModel> CreateService(CreateServiceViewModel model)
        {
            throw new System.NotImplementedException();
        }

        public string DeleteService(Service service)
        {
            throw new System.NotImplementedException();
        }

        public List<ServiceViewModel> GetByHouseId(string houseId)
        {
            throw new System.NotImplementedException();
        }

        public ServiceViewModel GetByID(string id)
        {
            throw new System.NotImplementedException();
        }

        public ServiceViewModel UpdateService(Service service, UpdateServiceViewModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
