using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
{
    public partial interface IClockCategoryRepository : IBaseRepository<ClockCategory>
    {
    }
    public partial class ClockCategoryRepository : BaseRepository<ClockCategory>, IClockCategoryRepository
    {
        public ClockCategoryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
