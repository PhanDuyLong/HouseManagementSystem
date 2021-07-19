using HMS.Data.Constants;
using HMS.Data.Parameters;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.Bill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// BillsController
    /// </summary>
    [Route("api/bills")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private IBillService _billService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billService"></param>
        public BillsController(IBillService billService)
        {
            _billService = billService;
        }

        /// <summary>
        /// Filter bills
        /// </summary>
        /// <param name="billParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ShowBillViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetBills([FromQuery] BillParameters billParameters)
        {
            var userId = User.Identity.Name;
            var bills = _billService.FilterByParameter(userId, billParameters);

            if (bills == null || bills.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "Bill" }).Value); ;

            return Ok(bills);
        }

        /// <summary>
        /// Get bill by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BillDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public IActionResult GetBill(int id)
        {
            var bill = _billService.GetById(id);
            if (bill == null) 
                return NotFound(new MessageResult("NF02", new string[] { "Bill"})); 

            return Ok(bill);
        }

        /// <summary>
        /// Create bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(BillDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _billService.CreateBillAsync(model);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(new MessageResult("BRO2", new string[] { "Bill" }).Value);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Update bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(BillDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(UpdateBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _billService.UpdateBillAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Confirm bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [Route("confirm")]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Confirm(ConfirmBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _billService.ConfirmBillAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }


        /// <summary>
        /// Delete bill
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _billService.DeleteBillAsync(id);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Count bills
        /// </summary>
        /// <param name="billParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.BadRequest)]
        public ActionResult Count([FromQuery] BillParameters billParameters)
        {
            var userId = User.Identity.Name;
            return Ok(_billService.CountBills(userId, billParameters));
        }
    }
}
