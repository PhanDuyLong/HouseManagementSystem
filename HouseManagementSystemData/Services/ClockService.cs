using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.Clock;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IClockService : IBaseService<Clock>
    {
        ClockDetailViewModel GetById(int id);
        List<ClockDetailViewModel> GetByRoomId(int roomId);
        Task<ResultResponse> CreateClockAsync(CreateClockViewModel model);
        Task<ResultResponse> DeleteClockAsync(int clockId);
        Task<ResultResponse> CreateAllClocksAsync(int roomId);
        int GetIdByServiceIdAndRoomId(int serviceId, int roomId);
        Task<ResultResponse> UpdateClockAsync(UpdateClockViewModel model);
    }
    public partial class ClockService : BaseService<Clock>, IClockService
    {
        private readonly IMapper _mapper;
        private readonly IClockCategoryService _clockCategoryService;
        private readonly IServiceService _serviceService;
        public ClockService(DbContext dbContext, IClockRepository repository, IMapper mapper
            , IClockCategoryService clockCategoryService, IServiceService serviceService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _clockCategoryService = clockCategoryService;
            _serviceService = serviceService;
        }

        public async Task<ResultResponse> CreateClockAsync(CreateClockViewModel model)
        {
            var clockCategories = await _clockCategoryService.GetClockCategoriesAsync();
            var clock = _mapper.Map<Clock>(model);
            clock.Status = ClockConstants.CLOCK_IS_ACTIVE;
            clock.ClockCategoryId = clockCategories.Where(category => category.Name.Equals(model.ClockCategoryName)).FirstOrDefault().Id;
            await CreateAsyn(clock);
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Clock" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> CreateAllClocksAsync(int roomId)
        {
            var clockCategories = await _clockCategoryService.GetClockCategoriesAsync();
            var clockList = new ArrayList();
            foreach(ClockCategory category in clockCategories)
            {
                var clock = new CreateClockViewModel
                {
                    RoomId = roomId,
                    ClockCategoryName = category.Name
                };
                clockList.Add(clock);
            }

            foreach (CreateClockViewModel model in clockList)
            {
                await CreateClockAsync(model);
            }
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "All clocks" }).Value,
                IsSuccess = true,
            };
        }

        public ClockDetailViewModel GetById(int id)
        {
            var clock = Get().Where(c => c.Id == id && c.Status == ClockConstants.CLOCK_IS_ACTIVE).ProjectTo<ClockDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return clock;
        }

        public List<ClockDetailViewModel> GetByRoomId(int roomId)
        {
            var clocks = Get().Where(c => c.RoomId == roomId && c.Status == ClockConstants.CLOCK_IS_ACTIVE).ProjectTo<ClockDetailViewModel>(_mapper.ConfigurationProvider).ToList();
            return clocks;
        }


        public int GetIdByServiceIdAndRoomId(int serviceId, int roomId)
        {
            var service = _serviceService.GetById(serviceId);
            var clocks = GetByRoomId(roomId);
            foreach (var clock in clocks)
            {
                if (clock.ClockCategory.ToUpper().Contains(service.Name.ToUpper()))
                {
                    return clock.Id;
                }
            }
            return -1;
        }

        public async Task<ResultResponse> DeleteClockAsync(int clockId)
        {
            var check = CheckClock(clockId);
            if (!check.IsSuccess)
            {
                return check;
            };

            var clock = await GetAsyn(clockId);
            clock.Status = ClockConstants.CLOCK_IS_INACTIVE;
            Update(clock);
            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Clock" }).Value,
                IsSuccess = true
            };
        }

        public ResultResponse CheckClock(int clockId)
        {
            var clock = GetById(clockId);
            if (clock == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Clock" }).Value,
                    IsSuccess = false
                };
            }
            return new ResultResponse
            {
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> UpdateClockAsync(UpdateClockViewModel model)
        {
            var check = CheckClock(model.Id);
            if (!check.IsSuccess)
            {
                return check;
            };

            var clock = await GetAsyn(model.Id);
            if (clock.Status != null)
            {
                clock.Status = model.Status;
            }
            if(clock.ClockCategory != null)
            {
                var clockCategories = await _clockCategoryService.GetClockCategoriesAsync();
                clock.ClockCategoryId = clockCategories.Where(category => category.Name.Equals(model.ClockCategory)).FirstOrDefault().Id;
            }

            Update(clock);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Clock" }).Value,
                IsSuccess = true
            };
        }
    }
}
