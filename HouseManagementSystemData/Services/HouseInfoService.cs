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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IHouseInfoService : IBaseService<HouseInfo>
    {
        HouseInfoViewModel GetByID(int id);
        Task<ResultResponse> UpdateHouseInfoAsync(int id, UpdateHouseInfoViewModel model);
    }
    public partial class HouseInfoService : BaseService<HouseInfo>, IHouseInfoService
    {
        private readonly IMapper _mapper;
        public HouseInfoService(DbContext dbContext, IHouseInfoRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public HouseInfoViewModel GetByID(int id)
        {
            var hInfo = Get().Where(hInfo => hInfo.Id == id).ProjectTo<HouseInfoViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return hInfo;
        }

        public async Task<ResultResponse> UpdateHouseInfoAsync(int id, UpdateHouseInfoViewModel model)
        {
            var houseInfo = await GetAsyn(id);
            houseInfo.Name = model.Name;
            houseInfo.Address = model.Address;
            houseInfo.Image = model.Image;
            Update(houseInfo);
            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "HouseInfo" }),
                IsSuccess = true,
            };
        }
    }
}
