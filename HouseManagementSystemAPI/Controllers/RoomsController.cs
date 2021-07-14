using HMS.Data.Constants;
using HMS.Data.Parameters;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Room;
using HMS.Data.ViewModels.RoomViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// RoomsController
    /// </summary>
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private IRoomService _roomService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roomService"></param>
        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Filter Rooms
        /// </summary>
        /// <param name="roomParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RoomShowViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetRooms([FromQuery] RoomParameters roomParameters)
        {
            var userId = User.Identity.Name;
            var rooms = _roomService.FilterByParameter(userId, roomParameters);
            if (rooms == null || rooms.Count == 0)
            {
                return NotFound(new MessageResult("NF01", new string[] { "Room" }).Value);
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
                return NotFound(new MessageResult("NF02", new string[] { "Room" }).Value);
            }
            return Ok(room);
        }

        /// <summary>
        /// Create Room
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(RoomDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(CreateRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roomService.CreateRoomAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update Room
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(UpdateRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roomService.UpdateRoomAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete Room
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _roomService.DeleteRoomAsync(id);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Count Rooms
        /// </summary>
        /// <param name="roomParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public ActionResult Count([FromQuery] RoomParameters roomParameters)
        {
            var userId = User.Identity.Name;
            return Ok(_roomService.CountRooms(userId, roomParameters));
        }
    }
}
