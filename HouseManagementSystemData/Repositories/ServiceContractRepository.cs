using HMS.Data.Models;
using HMS.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Repositories
{
    public partial interface IServiceContractRepository : IBaseRepository<ServiceContract>
    {
    }
    public partial class ServiceContractRepository : BaseRepository<ServiceContract>, IServiceContractRepository
    {
        public ServiceContractRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
