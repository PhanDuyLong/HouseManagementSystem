using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
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
