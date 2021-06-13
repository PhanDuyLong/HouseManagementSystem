using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Data.Services;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.RoomViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Get Rooms by HouseId
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        [HttpGet("{houseId}")]
        public List<RoomShowViewModel> GetRooms(String houseId)
        {
            return _roomService.GetByHouseID(houseId);
        }

        /// <summary>
        /// Get Room by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public RoomDetailViewModel GetRoom(int id)
        {
            return _roomService.GetByID(id);
        }

    }
}
