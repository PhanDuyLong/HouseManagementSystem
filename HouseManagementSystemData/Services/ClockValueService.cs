using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IClockValueService : IBaseService<ClockValue>
    {
        Task<ResultResponse> CreateClockValueAsync(CreateClockValueViewModel model);
    }
    public partial class ClockValueService : BaseService<ClockValue>, IClockValueService
    {
        private readonly IMapper _mapper;
        public ClockValueService(DbContext dbContext, IClockValueRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResultResponse> CreateClockValueAsync(CreateClockValueViewModel model)
        {
            await ResetClockMilestone(model.ClockId);
            var clockValue = _mapper.Map<ClockValue>(model);
            clockValue.Status = ClockValueConstants.CLOCK_VALUE_IS_MILESTONE;
            await CreateAsyn(clockValue);
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "ClockValue" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> ResetClockMilestone(int clockId)
        {
            var clockValueViewModels = Get().Where(value => value.ClockId == clockId && value.Status == ClockValueConstants.CLOCK_VALUE_IS_MILESTONE).ProjectTo<ClockValueDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            foreach(var value in clockValueViewModels)
            {
                var clockValue = await GetAsyn(value.Id);
                clockValue.Status = ClockValueConstants.CLOCK_VALUE_IS_NOT_MILESTONE;
                Update(clockValue);
            }

            return new ResultResponse
            {
                IsSuccess = true,
            };
        }
    }
}
