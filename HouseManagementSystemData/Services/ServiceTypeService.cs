using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Services
{
    public partial interface IServiceTypeService : IBaseService<ServiceType>
    {
    }
    public partial class ServiceTypeService : BaseService<ServiceType>, IServiceTypeService
    {
        private readonly IMapper _mapper;
        public ServiceTypeService(DbContext dbContext, IServiceTypeRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
