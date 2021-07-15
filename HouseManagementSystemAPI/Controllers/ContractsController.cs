using HMS.Data.Constants;
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
        /// Get Contracts by UserId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ContractBaseViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetContracts()
        {
            var userId = User.Identity.Name;
            var contracts = _contractService.GetByUserId(userId);
            if (contracts == null)
                return NotFound("Contract(s) is/are not found");

            return Ok(contracts);
        }

        /// <summary>
        /// Get Contract by Id 
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
                return NotFound("Contract is not found");
            }
            return Ok(contract);
        }

        /// <summary>
        /// Create Contract
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(ContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateContractViewModel model)
        {
            return Ok(await _contractService.CreateContractAsync(model));
        }

        /// <summary>
        /// Update Contract
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(ContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(UpdateContractViewModel model)
        {
            var contract = await _contractService.GetAsyn(model.Id);
            if (contract == null)
            {
                return NotFound("Contract is not found");
            }
            return Ok(_contractService.UpdateContractAsync(model));
        }

        /// <summary>
        /// Delete Contract
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
