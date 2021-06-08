using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Repositories
{
    public partial interface IAccountRepository : IBaseRepository<Account>
    {
    }
    public partial class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
