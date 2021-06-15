using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Services
{
    public partial interface IContractService : IBaseService<Contract>
    {
    }
    public partial class ContractService : BaseService<Contract>, IContractService
    {
        private readonly IMapper _mapper;
        public ContractService(DbContext dbContext, IContractRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
