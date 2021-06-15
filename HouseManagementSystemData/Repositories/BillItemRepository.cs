using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Repositories
{
    public partial interface IBillItemRepository : IBaseRepository<BillItem>
    {
    }
    public partial class BillItemRepository : BaseRepository<BillItem>, IBillItemRepository
    {
        public BillItemRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
