using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Repositories
{
    public partial interface IClockValueRepository : IBaseRepository<ClockValue>
    {
    }
    public partial class ClockValueRepository : BaseRepository<ClockValue>, IClockValueRepository
    {
        public ClockValueRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
