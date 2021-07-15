using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.HouseViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IHouseInfoService : IBaseService<HouseInfo>
    {
        HouseInfoViewModel GetById(int id);
        Task<ResultResponse> UpdateHouseInfoAsync(HouseBaseViewModel houseModel, UpdateHouseInfoViewModel model);
    }
    public partial class HouseInfoService : BaseService<HouseInfo>, IHouseInfoService
    {
        private readonly IMapper _mapper;
        public HouseInfoService(DbContext dbContext, IHouseInfoRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public HouseInfoViewModel GetById(int id)
        {
            var hInfo = Get().Where(hInfo => hInfo.Id == id).ProjectTo<HouseInfoViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return hInfo;
        }

        public async Task<ResultResponse> UpdateHouseInfoAsync(HouseBaseViewModel houseModel, UpdateHouseInfoViewModel model)
        {
            HouseInfo houseInfo;
            if(houseModel.HouseInfo == null)
            {
                houseInfo = new HouseInfo();
                houseInfo.HouseId = houseModel.Id;
            }
            else
            {
                houseInfo = await GetAsyn(houseModel.HouseInfo.Id);
            }
            if(model.Name != null)
            {
                houseInfo.Name = model.Name;

            }
            if(model.Address != null)
            {
                houseInfo.Address = model.Address;
            }
            if (model.Image != null)
            {
                houseInfo.Image = model.Image;
            }
            if(houseInfo.Id == 0)
            {
                await CreateAsyn(houseInfo);
            }
            Update(houseInfo);
            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "HouseInfo" }).Value,
                IsSuccess = true,
            };
        }
    }
}
