using HMS.Data.Requests;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        /// <summary>
        /// Authenticate login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("authenticate")]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _accountService.Authenticate(model);

            if (response == null)
                return NotFound("Username or password is incorrect");

            return Ok(response);
        }

        /// <summary>
        /// Get list of tenants'name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AccountTenantViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetTenantNames()
        {
            return Ok(_accountService.GetTenantNames());
        }

        /// <summary>
        /// Register Account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register(CreateAccountViewModel model)
        {
            var account = await _accountService.GetAsyn(model.Username);
            if (account != null)
                return Conflict("Username is existed");
            return Ok(await _accountService.CreateAccount(model));
        }

        /// <summary>
        /// Update profile
        /// </summary>
        /// <param name="username"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(AccountDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(string username, UpdateAccountViewModel model)
        {
            var account = await _accountService.GetAsyn(username);
            if (account == null)
                return NotFound("Account is not found");
            return Ok(_accountService.UpdateAccount(account, model));
        }


        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("change-password")]
        [HttpPut]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangePassword(string username, string password)
        {
            var account = await _accountService.GetAsyn(username);
            if (account == null)
                return NotFound("Account is not found");
            return Ok(_accountService.UpdatePassword(account, password));
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(string username)
        {
            var account = await _accountService.GetAsyn(username);
            if (account == null)
                return NotFound("Account is not found");
            return Ok(_accountService.DeleteAccount(account));
        }
    }
}
