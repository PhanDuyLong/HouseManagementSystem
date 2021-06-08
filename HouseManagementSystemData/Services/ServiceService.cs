using AutoMapper;
using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories;
using HouseManagementSystem.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Services
{
    public partial interface IServiceService : IBaseService<Service>
    {
    }
    public partial class ServiceService : BaseService<Service>, IServiceService
    {
        private readonly IMapper _mapper;
        public ServiceService(DbContext dbContext, IServiceRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
