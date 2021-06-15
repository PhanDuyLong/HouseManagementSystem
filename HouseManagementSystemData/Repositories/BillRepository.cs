using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
{
    public partial interface IBillRepository : IBaseRepository<Bill>
    {
    }
    public partial class BillRepository : BaseRepository<Bill>, IBillRepository
    {
        public BillRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
