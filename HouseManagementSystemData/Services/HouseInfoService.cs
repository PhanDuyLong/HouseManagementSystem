using AutoMapper;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Data.Services
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
