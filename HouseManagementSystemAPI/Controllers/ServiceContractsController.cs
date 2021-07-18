
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HMS.Data.Constants;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.ServiceContract;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// ServiceContractsController
    /// </summary>
    [Route("api/serviceContracts")]
    [ApiController]
    public class ServiceContractsController : ControllerBase
    {
        private IServiceContractService _serviceContractService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceContractService"></param>
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
        public IActionResult GetServiceContracts(int contractId)
        {
            var contracts = _serviceContractService.GetByContractId(contractId);

            if (contracts == null || contracts.Count == 0)
            {
                return NotFound(new MessageResult("NF01", new string[] { "Contract" }).Value);
            }
            return Ok(contracts);
        }


        /// <summary>
        /// Get ServiceContract by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetServiceContract(int id)
        {
            var contract = _serviceContractService.GetById(id);
            if (contract == null)
                return NotFound(new MessageResult("NF02", new string[] { "Contract" }).Value);
            return Ok(contract);
        }

        /// <summary>
        /// Create ServiceContract
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="contractId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(int roomId, int contractId, CreateServiceContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceContractService.CreateServiceContractAsync(roomId, contractId, model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }


        /// <summary>
        /// Update ServiceContract
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="contractId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)] 
        public async Task<IActionResult> Update(int roomId, int contractId, UpdateServiceContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceContractService.UpdateServiceContractAsync(roomId, contractId, model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);

                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete ServiceContract
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
                var result = await _serviceContractService.DeleteServiceContractAsync(id);
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
