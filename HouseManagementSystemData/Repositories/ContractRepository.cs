using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Repositories
{
    public partial interface IContractRepository : IBaseRepository<Contract>
    {
    }
    public partial class ContractRepository : BaseRepository<Contract>, IContractRepository
    {
        public ContractRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
