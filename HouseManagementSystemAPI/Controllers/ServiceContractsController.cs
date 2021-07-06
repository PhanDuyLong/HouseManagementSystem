using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HMS.Data.Services;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceContractsController : ControllerBase
    {
        private IServiceContractService _serviceContractService;

        public ServiceContractsController(IServiceContractService serviceContractService)
        {
            _serviceContractService = serviceContractService;
        }

        /// <summary>
        /// Get ServiceContracts by contractId
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ServiceContractDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetServiceContracts(int contractId)
        {
            var serviceContracts = _serviceContractService.GetByContractId(contractId);
            if (serviceContracts == null)
            {
                return NotFound("ServiceContract(s) is/are not found");
            }
            return Ok(serviceContracts);
        }


        /// <summary>
        /// Get ServiceContract by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetServiceContract(int id)
        {
            var serviceContract = _serviceContractService.GetByID(id);
            if (serviceContract == null)
            {
                return NotFound("ServiceContract is not found");
            }
            return Ok(serviceContract);
        }

        /// <summary>
        /// Update ServiceContract
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(UpdateServiceContractViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateServiceContractViewModel model)
        {
            var serviceContract = await _serviceContractService.GetAsyn(model.Id);
            if (serviceContract == null)
            {
                return NotFound("ServiceContract is not found");
            }
            return Ok(_serviceContractService.UpdateServiceContract(serviceContract, model));
        }

        /// <summary>
        /// Delete ServiceContract
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            var serviceContract = await _serviceContractService.GetAsyn(id);
            if (serviceContract == null)
            {
                return NotFound("ServiceContract is not found");
            }

            return Ok(_serviceContractService.DeleteServiceContract(serviceContract));
        }
    }
}
