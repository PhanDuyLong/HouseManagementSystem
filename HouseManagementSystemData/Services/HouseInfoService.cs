using AutoMapper;
using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories;
using HouseManagementSystem.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseManagementSystem.Data.Services
{
    public partial interface IHouseInfoService : IBaseService<HouseInfo>
    {
    }
    public partial class HouseInfoService : BaseService<HouseInfo>, IHouseInfoService
    {
        private readonly IMapper _mapper;
        public HouseInfoService(DbContext dbContext, IHouseInfoRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }
    }
}
