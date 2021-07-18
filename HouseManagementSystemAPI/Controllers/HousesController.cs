using HMS.Data.Constants;
using HMS.Data.Parameters;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.House;
using HMS.Data.ViewModels.HouseViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HouseManagementSystemAPI.Controllers
{
    /// <summary>
    /// HousesController
    /// </summary>
    [Route("api/houses")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly IHouseService _houseService;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="houseService"></param>
        public HousesController(IHouseService houseService)
        {
            _houseService = houseService;
        }

        /// <summary>
        /// Filter Houses
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<HouseDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public IActionResult GetHouses([FromQuery] HouseParameters houseParameters)
        {
            var userId = User.Identity.Name;
            var houses = _houseService.FilterByParameter(userId, houseParameters);
            if (houses == null || houses.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "House" }).Value);
            return Ok(houses);
        }

        /// <summary>
        /// Get House by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HouseDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetHouse(string id)
        {
            var house = _houseService.GetById(id);
            if (house == null)
            {
                return NotFound(new MessageResult("NF02", new string[] { "House" }).Value);
            }
            return Ok(house);
        }

        /// <summary>
        /// Create House
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateHouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var result = await _houseService.CreateHouseAsync(userId, model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update House
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(UpdateHouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _houseService.UpdateHouseAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete House
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete(string houseId)
        {
            if (ModelState.IsValid)
            {
                var result = await _houseService.DeleteHouseAsync(houseId);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Count Houses
        /// </summary>
        /// <param name="houseParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.BadRequest)]
        public ActionResult Count([FromQuery] HouseParameters houseParameters)
        {
            var userId = User.Identity.Name;
            return Ok(_houseService.CountHouses(userId, houseParameters));
        }
    }
}
