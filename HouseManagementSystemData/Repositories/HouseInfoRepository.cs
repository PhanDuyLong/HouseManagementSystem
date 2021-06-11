using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
{
    public partial interface IHouseInfoRepository : IBaseRepository<HouseInfo>
    {
    }
    public partial class HouseInfoRepository : BaseRepository<HouseInfo>, IHouseInfoRepository
    {
        public HouseInfoRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
