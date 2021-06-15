using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Repositories
{
    public partial interface IServiceTypeRepository : IBaseRepository<ServiceType>
    {
    }
    public partial class ServiceTypeRepository : BaseRepository<ServiceType>, IServiceTypeRepository
    {
        public ServiceTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
