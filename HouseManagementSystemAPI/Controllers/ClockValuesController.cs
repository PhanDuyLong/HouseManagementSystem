using HMS.Data.Constants;
using HMS.Data.Parameters;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Clock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// ClocksController
    /// </summary>
    [Route("api/clockValues")]
    [ApiController]
    public class ClockValuesController : ControllerBase
    {
        private IClockValueService _clockValueService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clockValueService"></param>
        public ClockValuesController(IClockValueService clockValueService)
        {
            _clockValueService = clockValueService;
        }

        /// <summary>
        /// Get clockValues by roomId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ClockDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public IActionResult GetClockValues([FromQuery] ClockValueParameter parameter) 
        {
            var clocks = _clockValueService.FilterByParameter(parameter);
            if (clocks == null || clocks.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "ClockValue" }).Value);
            return Ok(clocks);
        }

        /// <summary>
        /// Get clockValue by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClockDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetClockValue(int id)
        {
            var house = _clockValueService.GetById(id);
            if (house == null)
            {
                return NotFound(new MessageResult("NF02", new string[] { "ClockValue" }).Value);
            }
            return Ok(house);
        }

        /// <summary>
        /// Create clockValue
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateClockValueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _clockValueService.CreateClockValueAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update clockValue
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(UpdateClockValueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _clockValueService.UpdateClockValueAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete clockValue
        /// </summary>
        /// <param name="clockValueId"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete(int clockValueId)
        {
            if (ModelState.IsValid)
            {
                var result = await _clockValueService.DeleteClockValueAsync(clockValueId);
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
