using HMS.Data.Services;
using HMS.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// ServicesController
    /// </summary>
    [Route("api/services")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private IServiceService _serviceService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceService"></param>
        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Get Services by houseId
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ServiceViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetServices(string houseId)
        {
            var services = _serviceService.GetByHouseId(houseId);
            if (services == null)
            {
                return NotFound("Service(s) is/are not found");
            }
            return Ok(services);
        }

        /// <summary>
        /// Get Service by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HouseDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetHouse(int id)
        {
            var service = _serviceService.GetById(id);
            if (service == null)
                return NotFound("Service is not found");
            return Ok(service);
        }

        /// <summary>
        /// Create Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(CreateServiceViewModel model)
        {
            return Ok(await _serviceService.CreateService(model));
        }

        /// <summary>
        /// Update Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ServiceViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateServiceViewModel model)
        {
            var service = await _serviceService.GetAsyn(model.Id);
            if (service == null)
            {
                return NotFound("Service is not found");
            }
            return Ok(_serviceService.UpdateService(service, model));
        }

        /// <summary>
        /// Delete Contract
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            var service = await _serviceService.GetAsyn(id);
            if (service == null)
            {
                return NotFound("Service is not found");
            }

            return Ok(_serviceService.DeleteService(service));
        }
    }
}
