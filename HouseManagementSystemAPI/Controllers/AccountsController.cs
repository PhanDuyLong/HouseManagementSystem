using HMS.Data.Constants;
using HMS.Data.Requests;
using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// AccountsController
    /// </summary>
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accountService"></param>
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authenticate")]
        [ProducesResponseType(typeof(AuthenticateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
        {
            if(ModelState.IsValid)
            {
                var result = await _accountService.LoginAccountAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01"));
        }

        /// <summary>
        /// Authenticate with google
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authenticate-google")]
        [ProducesResponseType(typeof(AuthenticateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LoginByGoogle([FromBody] AuthenticateGoogleRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAccountByGoogleAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01"));
        }

        /// <summary>
        /// Register account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterAccountAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01"));
        }

        /// <summary>
        /// Register admin account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_ADMIN)]
        [HttpPost]
        [Route("register-admin")]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest model)
        {
            if(ModelState.IsValid)
            {
                var result = await _accountService.RegisterAccountAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            return BadRequest(new MessageResult("BR01"));
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
        /// Update profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(AccountDetailViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(UpdateAccountViewModel model)
        {
            var username = User.Identity.Name;
            return Ok(await _accountService.UpdateAccountAsync(username, model));
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Authorize(Roles = AccountConstants.ROLE_IS_ADMIN)]
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
