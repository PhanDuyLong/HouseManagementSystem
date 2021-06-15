using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Services
{
    public partial interface IServiceContractService : IBaseService<ServiceContract>
    {
    }
    public partial class ServiceContractService : BaseService<ServiceContract>, IServiceContractService
    {
        private readonly IMapper _mapper;
        public ServiceContractService(DbContext dbContext, IServiceContractRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
