using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.HouseViewModels;
using HMS.Data.ViewModels.Room;
using HMS.Data.ViewModels.RoomViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IRoomService : IBaseService<Room>
    {
        List<RoomShowViewModel> FilterByParameter(string userId, RoomParameters roomParameters);
        RoomDetailViewModel GetByID(int id);
        Task<RoomDetailViewModel> CreateRoom(CreateRoomViewModel model);
        RoomDetailViewModel UpdateRoom(Room room, UpdateRoomViewModel model);
        string DeleteRoom(Room room);
        int CountRooms(string userId, RoomParameters roomParameters);
    }
    public partial class RoomService : BaseService<Room>, IRoomService
    {
        private readonly IMapper _mapper;
        private readonly IHouseService _houseService;
        public RoomService(DbContext dbContext, IRoomRepository repository, IMapper mapper
            , IHouseService houseService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _houseService = houseService;
        }

        public List<RoomShowViewModel> FilterByParameter(string userId, RoomParameters roomParameters)
        {
            List<RoomShowViewModel> rooms;
            var houseId = roomParameters.HouseId;
            if (houseId != null)
            {
                rooms = GetByHouseId(roomParameters);
            }
            else
            {
                rooms = GetByUserId(userId, roomParameters);
            }
            var status = roomParameters.Status;
            if(status != null)
            {
                rooms = rooms.Where(r => r.Status == roomParameters.Status).ToList();
            }
            return rooms;
        }

        public List<RoomShowViewModel> GetByUserId(string userId, RoomParameters roomParameters)
        {
            List<RoomShowViewModel> rooms;
            var parameters = new HouseParameters();
            List<HouseBaseViewModel> houses = _houseService.FilterByParameter(userId, parameters);
            var houseIds = houses.Select(c => c.Id);
            rooms = Get().Where(r => houseIds.Contains(r.HouseId) && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomShowViewModel>(_mapper.ConfigurationProvider).ToList();
            return rooms;
        }

        public List<RoomShowViewModel> GetByHouseId(RoomParameters roomParameters)
        {
            List<RoomShowViewModel> rooms;
            rooms = Get().Where(r => r.HouseId == roomParameters.HouseId && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomShowViewModel>(_mapper.ConfigurationProvider).ToList();
            return rooms;
        }


        public RoomDetailViewModel GetByID(int id)
        {
            var room = Get().Where(r => r.Id == id && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return room;
        }

        string DeleteRoom(Room room)
        {
            throw new System.NotImplementedException();
        }

        public int CountRooms(string userId, RoomParameters roomParameters)
        {
            return FilterByParameter(userId, roomParameters).Count;
        }

        Task<RoomDetailViewModel> CreateRoom(CreateRoomViewModel model)
        {
            throw new System.NotImplementedException();
        }

        RoomDetailViewModel UpdateRoom(Room room, UpdateRoomViewModel model)
        {
            throw new System.NotImplementedException();
        }

        string IRoomService.DeleteRoom(Room room)
        {
            throw new System.NotImplementedException();
        }

        Task<RoomDetailViewModel> IRoomService.CreateRoom(CreateRoomViewModel model)
        {
            throw new System.NotImplementedException();
        }

        RoomDetailViewModel IRoomService.UpdateRoom(Room room, UpdateRoomViewModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
