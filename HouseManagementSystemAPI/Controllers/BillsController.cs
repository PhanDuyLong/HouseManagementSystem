using HMS.Data.Constants;
using HMS.Data.Parameters;
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
        /// Filter Bills
        /// </summary>
        /// <param name="billParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ShowBillViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetBills([FromQuery] BillParameters billParameters)
        {
            var username = User.Identity.Name;
            var bills = _billService.FilterByParameter(username, billParameters);

            if (bills == null || bills.Count == 0) 
                return NotFound("Bill(s) is/are not found");

            return Ok(bills);
        }


        /// <summary>
        /// Get Bill by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BillDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetBill(int id)
        {
            var bill = _billService.GetById(id);
            if (bill == null) 
                return NotFound(new MessageResult("NF02", new string[] { "Bill"})); 

            return Ok(bill);
        }

        /// <summary>
        /// Create Bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(BillDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string),(int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string),(int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(CreateBillViewModel model)
        {
            return Ok(await _billService.CreateBill(model));
        }

        /// <summary>
        /// Update Bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPut]
        [ProducesResponseType(typeof(BillDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateBillViewModel model)
        {
            var bill = await _billService.GetAsyn(model.Id);
            if (bill == null)
            {
                return NotFound("Bill is not found");
            }
            return Ok(_billService.UpdateBill(bill, model));
        }

        /// <summary>
        /// Delete Bill
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_OWNER + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            var bill = await _billService.GetAsyn(id);
            if (bill == null)
            {
                return NotFound("Bill is not found");
            }

            return Ok(_billService.DeleteBill(bill));
        }
    }
}
