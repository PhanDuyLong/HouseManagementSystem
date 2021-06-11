using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
{
    public partial interface IHouseRepository : IBaseRepository<House>
    {
    }
    public partial class HouseRepository : BaseRepository<House>, IHouseRepository
    {
        public HouseRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
