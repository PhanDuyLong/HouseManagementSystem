using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels.Clock;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HMS.Data.Services
{
    public partial interface IClockService : IBaseService<Clock>
    {
        ClockDetailViewModel GetById(string id);
    }
    public partial class ClockService : BaseService<Clock>, IClockService
    {
        private readonly IMapper _mapper;
        public ClockService(DbContext dbContext, IClockRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ClockDetailViewModel GetById(string id)
        {
            var clock = Get().Where(c => c.Id == id && c.Status == ClockConstants.CLOCK_IS_ACTIVE).ProjectTo<ClockDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return clock;
        }
    }
}
