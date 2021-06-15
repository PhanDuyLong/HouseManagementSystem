using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Repositories
{
    public partial interface IClockInContractRepository : IBaseRepository<ClockInContract>
    {
    }
    public partial class ClockInContractRepository : BaseRepository<ClockInContract>, IClockInContractRepository
    {
        public ClockInContractRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
