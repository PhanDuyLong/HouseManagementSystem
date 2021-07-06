using HMS.Data.Services;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Room;
using HMS.Data.ViewModels.RoomViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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
        [HttpGet]
        [ProducesResponseType(typeof(List<RoomShowViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetRooms(string houseId)
        {
            var rooms = _roomService.GetByHouseID(houseId);
            if (rooms == null)
            {
                return NotFound("Room(s) is/are not found");
            }
            return Ok(rooms);
        }

        /// <summary>
        /// Get Room by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoomDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetRoom(int id)
        {
            var room = _roomService.GetByID(id);
            if (room == null)
            {
                return NotFound("Room is not found");
            }
            return Ok(room);
        }

        /// <summary>
        /// Create Room
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(RoomDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(CreateRoomViewModel model)
        {
            return Ok(await _roomService.CreateRoom(model));
        }

        /// <summary>
        /// Update Room
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(RoomDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateRoomViewModel model)
        {
            var room = await _roomService.GetAsyn(model.Id);
            if (room == null)
            {
                return NotFound("Room is not found");
            }
            return Ok(_roomService.UpdateRoom(room, model));
        }

        /// <summary>
        /// Delete Room
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            var room = await _roomService.GetAsyn(id);
            if (room == null)
            {
                return NotFound("Room is not found");
            }

            return Ok(_roomService.DeleteRoom(room));
        }

    }
}
