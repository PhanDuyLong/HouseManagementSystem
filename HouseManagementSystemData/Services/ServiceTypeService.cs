using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IServiceTypeService : IBaseService<ServiceType>
    {
        Task<List<ServiceType>> GetServiceTypes();
    }
    public partial class ServiceTypeService : BaseService<ServiceType>, IServiceTypeService
    {
        private readonly IMapper _mapper;
        public ServiceTypeService(DbContext dbContext, IServiceTypeRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ServiceType>> GetServiceTypes()
        {
            return await Get().ToListAsync();
        }
    }
}
