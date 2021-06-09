using AutoMapper;
using AutoMapper.QueryableExtensions;
using HouseManagementSystem.Data.Models;
using HouseManagementSystem.Data.Repositories;
using HouseManagementSystem.Data.Services.Base;
using HouseManagementSystem.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HouseManagementSystem.Data.Services
{
    public partial interface IHouseService : IBaseService<House>
    {
        List<HouseViewModel> GetAllByOwnerUsername(string ownerUsername);
        HouseViewModel GetByID(string id);
    }
    public partial class HouseService : BaseService<House>, IHouseService
    {
        private readonly IMapper _mapper;
        public HouseService(DbContext dbContext, IHouseRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public List<HouseViewModel> GetAllByOwnerUsername(string ownerUsername)
        {
            var houses = Get().Where(h => h.OwnerUsername == ownerUsername).Include(h => h.HouseInfos).ProjectTo<HouseViewModel>(_mapper.ConfigurationProvider).ToList();
            return houses;
        }

        public HouseViewModel GetByID(string id)
        {
            var house = Get().Where(h => h.Id == id).Include(h => h.HouseInfos).FirstOrDefault();
            var houseViewModel = _mapper.Map<HouseViewModel>(house);
            return houseViewModel;
        }
    }
}
