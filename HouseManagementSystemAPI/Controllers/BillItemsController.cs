using HMS.Data.Constants;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.BillItem;
using HMS.Data.ViewModels.Clock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// BillItemsController
    /// </summary>
    [Route("api/billItems")]
    [ApiController]
    public class BillItemsController : ControllerBase
    {
        private IBillItemService _billItemService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billItemService"></param>
        public BillItemsController(IBillItemService billItemService)
        {
            _billItemService = billItemService;
        }

        /// <summary>
        /// Get billItems by billId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ClockDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public IActionResult GetBillItems([FromQuery] int billId)
        {
            var billItems = _billItemService.GetByBillId(billId);
            if (billItems == null || billItems.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "BillItem" }).Value);
            return Ok(billItems);
        }

        /// <summary>
        /// Get billItem by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClockDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetBillItem(int id)
        {
            var billItem = _billItemService.GetById(id);
            if (billItem == null)
            {
                return NotFound(new MessageResult("NF02", new string[] { "BillItem" }).Value);
            }
            return Ok(billItem);
        }

        /// <summary>
        /// Create billItem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateBillItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _billItemService.CreateBillItemAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update billItem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(UpdateBillItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _billItemService.UpdateBillItemAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Delete billItem
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete(int billId)
        {
            if (ModelState.IsValid)
            {
                var result = await _billItemService.DeleteBillItemAsync(billId);
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
