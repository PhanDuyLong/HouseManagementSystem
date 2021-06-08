using AutoMapper;
using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories;
using HouseManagementSystem.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Services
{
    public partial interface IAccountService : IBaseService<Account>
    {
        
    }
    public partial class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IMapper _mapper;
        public AccountService(DbContext dbContext, IAccountRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
