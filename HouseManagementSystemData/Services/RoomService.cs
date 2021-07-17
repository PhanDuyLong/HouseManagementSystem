using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Parameters;
using HMS.Data.Repositories;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Clock;
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
        RoomDetailViewModel GetById(int id);
        Task<ResultResponse> CreateRoomAsync(CreateRoomViewModel model);
        Task<ResultResponse> UpdateRoomAsync(UpdateRoomViewModel model);
        Task<ResultResponse> DeleteRoomAsync(int id);
        int CountRooms(string userId, RoomParameters roomParameters);
        Task<ResultResponse> UpdateRoomStatusAsync(int id, bool status);
        Task<ResultResponse> CheckInActiveRoomAsync(int id);
    }
    public partial class RoomService : BaseService<Room>, IRoomService
    {
        private readonly IMapper _mapper;
        private readonly IHouseService _houseService;
        private readonly IClockService _clockService;
        public RoomService(DbContext dbContext, IRoomRepository repository, IMapper mapper
            , IHouseService houseService, IClockService clockService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _houseService = houseService;
            _clockService = clockService;
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
            if (status != null)
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
            var rooms = Get().Where(r => r.HouseId == roomParameters.HouseId && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomShowViewModel>(_mapper.ConfigurationProvider).ToList();
            return rooms;
        }

        public RoomDetailViewModel GetById(int id)
        {
            var room = Get().Where(r => r.Id == id && r.IsDeleted == RoomConstants.ROOM_IS_NOT_DELETED).ProjectTo<RoomDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            if (room != null)
            {
                foreach (ClockDetailViewModel clock in room.Clocks)
                {
                    clock.ClockValues = clock.ClockValues.Where(value => value.Status == ClockValueConstants.CLOCK_VALUE_IS_MILESTONE).ToList();
                }
            }
            return room;
        }

        public int CountRooms(string userId, RoomParameters roomParameters)
        {
            return FilterByParameter(userId, roomParameters).Count;
        }

        public async Task<ResultResponse> CreateRoomAsync(CreateRoomViewModel model)
        {
            var room = _mapper.Map<Room>(model);
            room.Status = RoomConstants.ROOM_IS_NOT_RENTED;
            room.IsDeleted = RoomConstants.ROOM_IS_NOT_DELETED;
            await CreateAsyn(room);
            await _clockService.CreateAllClocksAsync(room.Id);

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Room" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> UpdateRoomAsync(UpdateRoomViewModel model)
        {
            var roomModel = GetById(model.Id);
            if (roomModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Room" }).Value,
                    IsSuccess = false
                };
            }

            var room = await GetAsyn(model.Id);
            if (model.Name != null)
            {
                room.Name = model.Name;
            }

            Update(room);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Room" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> DeleteRoomAsync(int id)
        {
            var check = await CheckInActiveRoomAsync(id);
            if (!check.IsSuccess)
            {
                return check;
            }

            var room = await GetAsyn(id);
            room.IsDeleted = RoomConstants.ROOM_IS_DELETED;
            Update(room);

            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "Room" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> UpdateRoomStatusAsync(int id, bool status)
        {
            var room = await GetAsyn(id);
            if (room == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Room" }).Value,
                    IsSuccess = false
                };
            }

            if (room.Status != status)
            {
                room.Status = status;
                Update(room);

                var house = _houseService.GetById(room.HouseId);

                if (status == RoomConstants.ROOM_IS_RENTED)
                {
                    await _houseService.UpdateHouseStatusAsync(house.Id, HouseConstants.HOUSE_IS_RENTED);
                }
                else
                {
                    house.Rooms = house.Rooms.Where(r => r.Status != status).ToList();
                    if (house.Rooms.Count == 0)
                    {
                        await _houseService.UpdateHouseStatusAsync(house.Id, HouseConstants.HOUSE_IS_NOT_RENTED);
                    }
                }

                return new ResultResponse
                {
                    Message = new MessageResult("OK03", new string[] { "RoomStatus" }).Value,
                    IsSuccess = true
                };
            }

            return new ResultResponse
            {
                Message = "Nothing change!",
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> CheckInActiveRoomAsync(int id)
        {
            var room = await GetAsyn(id);
            if (room == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Room" }).Value,
                    IsSuccess = false
                };
            }

            if (room.Status == RoomConstants.ROOM_IS_RENTED)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR04", new string[] { "Room", "rented" }).Value,
                    IsSuccess = false
                };
            }

            return new ResultResponse
            {
                IsSuccess = false
            };
        }
    }
}
