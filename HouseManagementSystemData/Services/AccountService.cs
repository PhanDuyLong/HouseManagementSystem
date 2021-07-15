using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HMS.Authen.Models;
using HMS.Authen.Services;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Requests;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IAccountService : IBaseService<Account>
    {
        Task<AuthenticateResponse> LoginAccountAsync(AuthenticateRequest request);
        Task<AuthenticateResponse> LoginAccountByGoogleAsync(AuthenticateGoogleRequest model);
        Task<ResultResponse> RegisterAccountAsync(RegisterRequest model);
        Task<ResultResponse> RegisterAdminAccountAsync(RegisterRequest model);
        AccountDetailViewModel GetByUserId(string userId);
        AccountDetailViewModel GetByEmail(string email);
        List<AccountTenantViewModel> GetTenantNames();
        Task<ResultResponse> UpdateAccountAsync(string userId, UpdateAccountViewModel model);
        string DeleteAccount(Account account);
    }
    public partial class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IMapper _mapper;
        private readonly IAccountAuthenService _accountAuthenService;
        public AccountService(DbContext dbContext, IAccountRepository repository, IMapper mapper
            , IAccountAuthenService accountAuthenService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _accountAuthenService = accountAuthenService;
        }

        public string DeleteAccount(Account account)
        {
            account.Status = AccountConstants.ACCOUNT_IS_INACTIVE;
            Update(account);
            return "Deleted succesfully";
        }

        public AccountDetailViewModel GetByUserId(string userId)
        {
            var account = Get().Where(a => a.UserId == userId).FirstOrDefault();

            var accountViewModel = _mapper.Map<AccountDetailViewModel>(account);

            return accountViewModel;
        }

        public AccountDetailViewModel GetByEmail(string email)
        {
            var account = Get().Where(a => a.Email == email).FirstOrDefault();

            var accountViewModel = _mapper.Map<AccountDetailViewModel>(account);

            return accountViewModel;
        }

        public List<AccountTenantViewModel> GetTenantNames()
        {
            var tenants = Get().Where(a => a.Role == AccountConstants.ROLE_IS_TENACT).ProjectTo<AccountTenantViewModel>(_mapper.ConfigurationProvider).ToList();
            return tenants;
        }

        public async Task<AuthenticateResponse> LoginAccountAsync(AuthenticateRequest model)
        {
            var internalRequest = new AuthenticateInternalRequest
            {
                Email = model.Email,
                Password = model.Password
            };

            var result = await _accountAuthenService.LoginAccountAsync(internalRequest);

            if (!result.IsSuccess)
            {
                return new AuthenticateResponse
                {
                    Message = result.Message,
                    IsSuccess = false,
                };
            }

            var accountViewModel = GetByEmail(model.Email);

            var firebaseCheck = CustomFirebaseAuth(accountViewModel.UserId, model.Email, model.Password);

            if (!firebaseCheck)
            {
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR06", new string[] { "email" }).Value,
                    IsSuccess = false,
                };
            }

            var message = new MessageResult("OK04");

            return new AuthenticateResponse(accountViewModel, result.Token, message, true, result.ExpireDate);
        }

        public async Task<ResultResponse> RegisterAccountAsync(RegisterRequest model)
        {
            var check = await CheckValidUserIdAndEmailAsync(model.UserId, model.Email);
            if (!check.IsSuccess)
                return check;

            check = CheckValidRole(model.Role);
            if (!check.IsSuccess)
                return check;

            var registerAccountRequest = new RegisterAccountRequest
            {
                UserId = model.UserId,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
            };

            var result = await _accountAuthenService.RegisterAccountAsync(registerAccountRequest);

            if (!result.IsSuccess)
            {
                if(result.Errors != null)
                {
                    return new ResultResponse
                    {
                        Message = new MessageResult("BRO3", new string[] { "Account" }).Value + "\n" + result.GetErrors(),
                        IsSuccess = false,
                    };
                }

                return new ResultResponse
                {
                    Message = result.Message,
                    IsSuccess = false,
                };
            }

            var acc = _mapper.Map<Account>(model);
            acc.Password = null;
            await CreateAsyn(acc);

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Account" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> RegisterAdminAccountAsync(RegisterRequest model)
        {
            var check = await CheckValidUserIdAndEmailAsync(model.UserId, model.Email);

            if (!check.IsSuccess)
                return check;

            model.Role = AccountConstants.ROLE_IS_ADMIN;

            var registerAccountRequest = new RegisterAccountRequest
            {
                UserId = model.UserId,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
            };

            var result = await _accountAuthenService.RegisterAccountAsync(registerAccountRequest);

            if (!result.IsSuccess)
            {
                if (result.Errors != null)
                {
                    return new ResultResponse
                    {
                        Message = new MessageResult("BR02", new string[] { "Account" }).Value + "\n" + result.GetErrors(),
                        IsSuccess = false,
                    };
                }

                return new ResultResponse
                {
                    Message = result.Message,
                    IsSuccess = false,
                };
            }

            var acc = _mapper.Map<Account>(model);
            acc.Password = null;
            await CreateAsyn(acc);

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Account" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> UpdateAccountAsync(string userId, UpdateAccountViewModel model)
        {
            var account = await GetAsyn(userId);
            if (account == null || account.Status == AccountConstants.ACCOUNT_IS_INACTIVE)
            {
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR06", new string[] { "userId" }).Value,
                    IsSuccess = false,
                };
            }
            
            if (model.Name != null)
            {
                account.Name = model.Name;
            }

            if (model.Phone != null)
            {
                account.Phone = model.Phone;
            }

            if (model.Image != null)
            {
                account.Image = model.Image;
            }

            Update(account);

            return new ResultResponse
            {
                Message = new MessageResult("OK03", new string[] { "Account's profile" }).Value,
                IsSuccess = true
            }; ;
        }

        public async Task<AuthenticateResponse> LoginAccountByGoogleAsync(AuthenticateGoogleRequest model)
        {
            /*var account = await _accountManager.FindByNameAsync(model.UserId);
            var acc = GetByUserId(model.UserId);
            if (account == null || acc == null)
            {
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR06", new string[] { "userId" }).Value,
                    IsSuccess = false,
                };
            }

            var accountRoles = await _accountManager.GetRolesAsync(account);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var accountRole in accountRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, accountRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );


            await GoogleFirebaseAuthAsync(model.UserId);


            var accountViewModel = GetByUserId(account.UserName);
            var message = new MessageResult("OK04");
            return new AuthenticateResponse(accountViewModel, new JwtSecurityTokenHandler().WriteToken(token), message, true, token.ValidTo);*/
            return null;
        }

        public async Task<ResultResponse> CheckValidUserIdAndEmailAsync(string userId, string email)
        {
            var accountExistsWithId = await GetAsyn(userId);
            var accountExistsWithEmail = GetByEmail(email);

            if (accountExistsWithId != null || accountExistsWithEmail != null)
                return new ResultResponse
                {
                    Message = new MessageResult("BR03", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };
            if (accountExistsWithId.Status.Equals(AccountConstants.ACCOUNT_IS_INACTIVE))
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR04", new string[] { "Account", "inactive"}).Value,
                    IsSuccess = false,
                };
            }
            return new ResultResponse
            {
                IsSuccess = true
            };
        }

        public bool CustomFirebaseAuth(string userId, string email, string password)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + "AIzaSyBOUnY4MomlWzp-8pMw2QNStK-k6Q27FB4");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    email = email,
                    password = password,
                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                using WebResponse tResponse = tRequest.GetResponse();
                using Stream dataStreamResponse = tResponse.GetResponseStream();
                string sResponseFromServer = "";
                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                    {
                        sResponseFromServer = tReader.ReadToEnd();
                    }
                if (sResponseFromServer != null && sResponseFromServer.Length != 0)
                {
                    var json = JObject.Parse(sResponseFromServer); ;
                    string firebaseUserId = (string)json.SelectToken("localId");
                    return firebaseUserId.Equals(userId);
                }
            }
            catch (Exception ex)
            {
                if(ex != null)
                    return false;
            }
            return false;
        }

        public ResultResponse CheckValidRole(string role)
        {
            if (AccountConstants.ROLE_IS_TENACT.Equals(role) || AccountConstants.ROLE_IS_OWNER.Equals(role) || AccountConstants.ROLE_IS_ADMIN.Equals(role))
            {
                return new ResultResponse
                {
                    IsSuccess = true
                };   
            }
            return new ResultResponse
            {
                Message = new MessageResult("BRO7", new string[] { "role" }).Value,
                IsSuccess = false,
            };
        }
    }
}
