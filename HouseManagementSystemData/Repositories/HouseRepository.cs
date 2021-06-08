using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Repositories
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
