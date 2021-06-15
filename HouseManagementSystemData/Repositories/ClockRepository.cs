using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Repositories
{
    public partial interface IClockRepository : IBaseRepository<Clock>
    {
    }
    public partial class ClockRepository : BaseRepository<Clock>, IClockRepository
    {
        public ClockRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
