using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Services
{
    public partial interface IBillService : IBaseService<Bill>
    {
    }
    public partial class BillService : BaseService<Bill>, IBillService
    {
        private readonly IMapper _mapper;
        public BillService(DbContext dbContext, IBillRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
