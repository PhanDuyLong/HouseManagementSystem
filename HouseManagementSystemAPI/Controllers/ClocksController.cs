using HMS.Data.Constants;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.Clock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// ClocksController
    /// </summary>
    [Route("api/clocks")]
    [ApiController]
    public class ClocksController : ControllerBase
    {
        private IClockService _clockService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clockService"></param>
        public ClocksController(IClockService clockService)
        {
            _clockService = clockService;
        }

        /// <summary>
        /// Get clocks by roomId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ClockDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public IActionResult GetClocks([FromQuery] int roomId)
        {
            var clocks = _clockService.GetByRoomId(roomId);
            if (clocks == null || clocks.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "Clock" }).Value);
            return Ok(clocks);
        }

        /// <summary>
        /// Get clock by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClockDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetClock(int id)
        {
            var house = _clockService.GetById(id);
            if (house == null)
            {
                return NotFound(new MessageResult("NF02", new string[] { "Clock" }).Value);
            }
            return Ok(house);
        }

        /// <summary>
        /// Create clock
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateClockViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _clockService.CreateClockAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update clock
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(UpdateClockViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _clockService.UpdateClockAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete clock
        /// </summary>
        /// <param name="clockId"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete(int clockId)
        {
            if (ModelState.IsValid)
            {
                var result = await _clockService.DeleteClockAsync(clockId);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }
    }
}
