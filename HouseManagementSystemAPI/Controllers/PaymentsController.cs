using HMS.Data.Services;
using HMS.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Pay bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Payment(BillPaymentViewModel model)
        {
            var bill = await _paymentService.GetAsyn(model.BillId);
            if (bill == null)
                return NotFound("Bill is not found");
            return Ok(await _paymentService.Payment(model));
        }

        /// <summary>
        /// Approve/Deny payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /*[HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> (BillPaymentViewModel model)
        {
            var bill = await _paymentService.GetAsyn(model.BillId);
            if (bill == null)
                return NotFound("Bill is not found");
            return Ok(await _paymentService.Payment(model));
        }*/
    }
}
