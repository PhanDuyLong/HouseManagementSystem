using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
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
