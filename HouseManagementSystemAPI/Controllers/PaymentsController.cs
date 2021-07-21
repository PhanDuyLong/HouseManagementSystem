using HMS.Data.Constants;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// PaymentsController
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private IPaymentService _paymentService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentService"></param>
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Pay bill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_TENACT + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [Route("confirm")]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Payment(PayBillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _paymentService.Payment(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01").Value);
        }

        /// <summary>
        /// Confirm Payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_TENACT + "," + AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> ConfirmPayment(ConfirmPaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _paymentService.ConfirmPayment(model);
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
