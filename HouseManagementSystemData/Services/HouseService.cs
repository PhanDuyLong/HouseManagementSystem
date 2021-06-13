using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using HMS.Data.ViewModels.HouseViewModels;
using HMS.Data.ViewModels;
using HMS.Data.Constants;

namespace HMS.Data.Services
{
    public partial interface IHouseService : IBaseService<House>
    {
        List<HouseBaseViewModel> GetByOwnerUsername(string ownerUsername);
        HouseDetailViewModel GetByID(string id);
    }
    public partial class HouseService : BaseService<House>, IHouseService
    {
        private readonly IMapper _mapper;
        public HouseService(DbContext dbContext, IHouseRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public List<HouseBaseViewModel> GetByOwnerUsername(string ownerUsername)
        {
            var houses = Get().Where(h => h.OwnerUsername == ownerUsername && h.IsDeleted == HouseConstants.HOUSE_IS_NOT_DELETED).ProjectTo<HouseBaseViewModel>(_mapper.ConfigurationProvider).ToList();
            return houses;
        }

        public HouseDetailViewModel GetByID(string id)
        {
            var house = Get().Where(h => h.Id == id && h.IsDeleted == HouseConstants.HOUSE_IS_NOT_DELETED).ProjectTo<HouseDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return house;
        }
    }
}