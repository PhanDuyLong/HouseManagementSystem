using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.RoomViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HMS.Data.Services
{
    public partial interface IRoomService : IBaseService<Room>
    {
        List<RoomShowViewModel> GetByHouseID(string id);
        RoomDetailViewModel GetByID(int id);
    }
    public partial class RoomService : BaseService<Room>, IRoomService
    {
        private readonly IMapper _mapper;
        public RoomService(DbContext dbContext, IRoomRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public List<RoomShowViewModel> GetByHouseID(string id)
        {
            var rooms = Get().Where(r => r.HouseId == id && r.Status == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomShowViewModel>(_mapper.ConfigurationProvider).ToList();
            return rooms;
        }

        public RoomDetailViewModel GetByID(int id)
        {
            var room = Get().Where(r => r.Id == id && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return room;
        }
    }
}
