using HMS.Data.Constants;
using HMS.Data.Parameters;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.Contract;
using HMS.Data.ViewModels.Contract.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// ContractsController
    /// </summary>
    [Route("api/contracts")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private IContractService _contractService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contractService"></param>
        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        /// <summary>
        /// Filter contracts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ContractDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetContracts(ContractParameters contractParameter)
        {
            var userId = User.Identity.Name;
            var contracts = _contractService.FilterByParamter(userId, contractParameter);
            if (contracts == null || contracts.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "Contract" }).Value);

            return Ok(contracts);
        }

        /// <summary>
        /// Get contract by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetContract(int id)
        {
            var contract = _contractService.GetById(id);
            if (contract == null)
            {
                return NotFound(new MessageResult("NF02", new string[] { "Contract" }).Value);
            }
            return Ok(contract);
        }

        /// <summary>
        /// Create contract
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateContractViewModel model)
        { 
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var result = await _contractService.CreateContractAsync(userId, model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update contract
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(UpdateContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _contractService.UpdateContractAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete contract
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Delete(int contractId)
        {
            if (ModelState.IsValid)
            {
                var result = await _contractService.DeleteContractAsync(contractId);
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
