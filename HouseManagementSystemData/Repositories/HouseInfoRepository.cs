using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Repositories
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
