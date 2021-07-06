using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HMS.Data.Services;
using HMS.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers
{
    [Route("api/houseInfos")]
    [ApiController]
    public class HouseInfosController : ControllerBase
    {
        private IHouseInfoService _houseInfoService;
        private IHouseService _houseService;

        public HouseInfosController(IHouseInfoService houseInfoService, IHouseService houseService)
        {
            _houseInfoService = houseInfoService;
            _houseService = houseService;
        }

        /// <summary>
        /// Update HouseInfo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(HouseInfoViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateHouseInfoViewModel model)
        {
            var house = await _houseService.GetAsyn(model.HouseId);
            if (house == null)
            {
                return NotFound("House is not found");
            }
            var houseInfo = await _houseInfoService.GetAsyn(model.Id);
            return Ok(_houseInfoService.UpdateHouseInfo(houseInfo, model));
        }
    }
}
