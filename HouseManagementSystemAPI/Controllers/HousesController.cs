using HMS.Data.Models;
using HMS.Data.Services;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.HouseViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HouseManagementSystemAPI.Controllers
{
    [Route("api/houses/")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly HMSDBContext _context;
        private IHouseService _houseService;
        public HousesController(HMSDBContext context, IHouseService houseService)
        {
            _context = context;
            _houseService = houseService;
        }

        /// <summary>
        /// Get Houses by ownerUsername
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<HouseDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetHouses(string username)
        {
            var houses = _houseService.GetByOwnerUsername(username);
            if(houses == null)
            {
                return NotFound("House(s) is/are not found");
            }
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetHouse(string id)
        {
            var house = _houseService.GetByID(id);
            if (house == null)
            {
                return NotFound("House is not found");
            }
            return Ok(house);
        }

        /// <summary>
        /// Create House
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(HouseDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(CreateHouseViewModel model)
        {
            return Ok(await _houseService.CreateHouse(model));
        }



        /// <summary>
        /// Delete House
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            var house = await _houseService.GetAsyn(id);
            if (house == null)
            {
                return NotFound("Room is not found");
            }

            return Ok(_houseService.DeleteHouse(house));
        }
    }
}
