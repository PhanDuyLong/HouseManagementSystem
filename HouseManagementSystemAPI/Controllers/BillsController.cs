using HMS.Data.Attributes;
using HMS.Data.Services;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Bill;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    [Route("api/bills")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;
        }

        /// <summary>
        /// Get Bills by contractId
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [HttpGet("{contractId}")]
        [ProducesResponseType(typeof(List<BillDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetBills(int contractId)
        {
            var bills = _billService.GetByContractID(contractId);

            if (bills == null) 
                return NotFound("Bill(s) is/are not found");

            return Ok(bills);
        }

        /// <summary>
        /// Get Bill by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BillDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetBillsByUsername(string username)
        {
            var bills = _billService.GetByUsername(username);

            if (bills == null)
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
            var bill = _billService.GetByID(id);

            if (bill == null) 
                return NotFound("Bill is not found"); 

            return Ok(bill);
        }

        /// <summary>
        /// Create Bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
