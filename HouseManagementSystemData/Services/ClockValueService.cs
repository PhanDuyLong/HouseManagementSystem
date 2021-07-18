using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Clock;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IClockValueService : IBaseService<ClockValue>
    {
        Task<ResultResponse> CreateClockValueAsync(CreateClockValueViewModel model);
        Task<ResultResponse> DeleteClockValueAsync(int clockValueId);
        List<ClockValueDetailViewModel> FilterByParameter(ClockValueParameter parameter);
        Task<ResultResponse> UpdateClockValueAsync(UpdateClockValueViewModel model);
        ClockValueDetailViewModel GetById(int id);
        ResultResponse CheckClockValue(int clockValueId);
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

        public async Task<ResultResponse> DeleteClockValueAsync(int clockValueId)
        {
            var check = CheckClockValue(clockValueId);
            if (!check.IsSuccess)
            {
                return check;
            }

            var clockValue = await GetAsyn(clockValueId);
            clockValue.Status = ClockValueConstants.CLOCK_VALUE_IS_NOT_MILESTONE;
            Update(clockValue);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "ClockValue" }).Value,
                IsSuccess = true
            };
        }

        public List<ClockValueDetailViewModel> FilterByParameter(ClockValueParameter parameter)
        {
            var clockValues = Get().Where(value => value.ClockId == parameter.ClockId).ProjectTo<ClockValueDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            if(clockValues != null)
            {
                if (parameter.IsRecordDateAscending)
                {
                    clockValues = clockValues.OrderBy(b => b.RecordDate).ToList();
                }
                else
                {
                    clockValues = clockValues.OrderByDescending(b => b.RecordDate).ToList();
                }
                var status = parameter.Status;
                if (status != null)
                {
                    clockValues = clockValues.Where(h => h.Status == parameter.Status).ToList();
                }
            }
            return clockValues;
        }

        public ClockValueDetailViewModel GetById(int id)
        {
            var clockValue = Get().Where(value => value.Id == id).ProjectTo<ClockValueDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return clockValue;
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

        public ResultResponse CheckClockValue(int clockValueId)
        {
            var clockValue = GetById(clockValueId);
            if (clockValue == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "ClockValue" }).Value,
                    IsSuccess = false
                };
            }
            return new ResultResponse
            {
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> UpdateClockValueAsync(UpdateClockValueViewModel model)
        {
            var check = CheckClockValue(model.Id);
            if (!check.IsSuccess)
            {
                return check;
            }

            var clockValue = await GetAsyn(model.Id);
            if(model.CreateDate != null)
            {
                clockValue.CreateDate = model.CreateDate;
            }
            if(model.RecordDate != null)
            {
                clockValue.RecordDate = model.RecordDate;
            }
            if(model.IndexValue != null)
            {
                clockValue.IndexValue = model.IndexValue;
            }
            if (clockValue.Status != null)
            {
                clockValue.Status = model.Status;
            }

            Update(clockValue);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "ClockValue" }).Value,
                IsSuccess = true
            };
        }
    }
}
