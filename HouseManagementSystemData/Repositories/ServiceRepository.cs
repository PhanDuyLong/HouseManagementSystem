using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Repositories
{
    public partial interface IServiceRepository : IBaseRepository<Service>
    {
    }
    public partial class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
