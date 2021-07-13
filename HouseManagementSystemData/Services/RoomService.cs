using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
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
        List<RoomShowViewModel> FilterByParameter(RoomParameters roomParameters);
        RoomDetailViewModel GetByID(int id);
        Task<RoomDetailViewModel> CreateRoom(CreateRoomViewModel model);
        RoomDetailViewModel UpdateRoom(Room room, UpdateRoomViewModel model);
        string DeleteRoom(Room room);
        int CountRooms(RoomParameters roomParameters);
    }
    public partial class RoomService : BaseService<Room>, IRoomService
    {
        private readonly IMapper _mapper;
        public RoomService(DbContext dbContext, IRoomRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<RoomShowViewModel> FilterByParameter(RoomParameters roomParameters)
        {
            var rooms = Get().Where(r => r.HouseId == roomParameters.HouseId && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomShowViewModel>(_mapper.ConfigurationProvider).ToList();
            var status = roomParameters.Status;
            if(status != null)
            {
                rooms = rooms.Where(r => r.Status == roomParameters.Status).ToList();
            }
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

        public int CountRooms(RoomParameters roomParameters)
        {
            return FilterByParameter(roomParameters).Count;
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
