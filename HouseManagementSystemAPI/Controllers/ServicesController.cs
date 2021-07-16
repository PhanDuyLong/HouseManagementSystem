using HMS.Data.Constants;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        public IActionResult GetServices(string houseId)
        {
            var services = _serviceService.GetByHouseId(houseId);
           
            if (services == null || services.Count == 0)
            {
                return NotFound(new MessageResult("NF01", new string[] { "Service" }).Value);
            }
            return Ok(services);
        }

        /// <summary>
        /// Get Service by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetService(int id)
        {
            var service = _serviceService.GetById(id);
            if (service == null)
                return NotFound(new MessageResult("NF02", new string[] { "Service" }).Value);
            return Ok(service);
        }

        /// <summary>
        /// Create Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceService.CreateServiceAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(UpdateServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceService.UpdateServiceAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete Service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceService.DeleteServiceAsync(id);
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
