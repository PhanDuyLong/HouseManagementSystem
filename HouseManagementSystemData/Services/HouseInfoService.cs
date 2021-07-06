using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HMS.Data.Services
{
    public partial interface IHouseInfoService : IBaseService<HouseInfo>
    {
        HouseInfoViewModel GetByID(int id);
        HouseInfoViewModel UpdateHouseInfo(HouseInfo houseInfo, UpdateHouseInfoViewModel model);
    }
    public partial class HouseInfoService : BaseService<HouseInfo>, IHouseInfoService
    {
        private readonly IMapper _mapper;
        public HouseInfoService(DbContext dbContext, IHouseInfoRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public HouseInfoViewModel GetByID(int id)
        {
            var hInfo = Get().Where(hInfo => hInfo.Id == id).ProjectTo<HouseInfoViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return hInfo;
        }

        public HouseInfoViewModel UpdateHouseInfo(HouseInfo houseInfo, UpdateHouseInfoViewModel model)
        {
            houseInfo.Name = model.Name;
            houseInfo.Address = model.Address;
            Update(houseInfo);
            return GetByID(houseInfo.Id);
        }
    }
}
