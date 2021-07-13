using HMS.Data.Services;
using HMS.Data.ViewModels.Contract;
using HMS.Data.ViewModels.Contract.Base;
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetContract(int id)
        {
            var contract = _contractService.GetByID(id);
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
        [HttpPost]
        [ProducesResponseType(typeof(ContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(CreateContractViewModel model)
        {
            return Ok(await _contractService.CreateContract(model));
        }

        /// <summary>
        /// Update Contract
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ContractDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateContractViewModel model)
        {
            var contract = await _contractService.GetAsyn(model.Id);
            if (contract == null)
            {
                return NotFound("Contract is not found");
            }
            return Ok(_contractService.UpdateContract(contract, model));
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
            var house = await _contractService.GetAsyn(id);
            if (house == null)
            {
                return NotFound("Room is not found");
            }

            return Ok(_contractService.DeleteContract(house));
        }
    }
}
