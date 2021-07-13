using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;


namespace HMS.Data.Services
{
    public partial interface IClockValueService : IBaseService<ClockValue>
    {
    }
    public partial class ClockValueService : BaseService<ClockValue>, IClockValueService
    {
        private readonly IMapper _mapper;
        public ClockValueService(DbContext dbContext, IClockValueRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
