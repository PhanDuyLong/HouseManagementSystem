using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;
namespace HMS.Data.Services
{
    public partial interface IClockService : IBaseService<Clock>
    {
    }
    public partial class ClockService : BaseService<Clock>, IClockService
    {
        private readonly IMapper _mapper;
        public ClockService(DbContext dbContext, IClockRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
