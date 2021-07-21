using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HMS.Authen.Models;
using HMS.Authen.Requests;
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
using HMS.FirebaseServices.Authen.Requests;
using HMS.FirebaseServices.Authen.Services;
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
        AccountDetailViewModel GetByUserId(string userId);
        AccountDetailViewModel GetByEmail(string email);
        List<AccountTenantViewModel> GetTenantNames();
        Task<ResultResponse> UpdateAccountAsync(string userId, UpdateAccountViewModel model);
        Task<ResultResponse> ChangePasswordAsync(string userId, ChangePassRequest model);
        Task<ResultResponse> CheckValidUserAsync(string userId);
    }
    public partial class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IMapper _mapper;
        private readonly IAccountAuthenService _accountAuthenService;
        private readonly IFirebaseAuthenService _firebaseAuthenService;
        public AccountService(DbContext dbContext, IAccountRepository repository, IMapper mapper
            , IAccountAuthenService accountAuthenService, IFirebaseAuthenService firebaseAuthenService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _accountAuthenService = accountAuthenService;
            _firebaseAuthenService = firebaseAuthenService;
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
            var firebaseResponse = await _firebaseAuthenService.LoginByIdTokenAsync(model.IdToken);

            if (!firebaseResponse.IsSuccess)
            {
                return new AuthenticateResponse
                {
                    Message = firebaseResponse.Message,
                    IsSuccess = false,
                };
            }

            var internalRequest = new AuthenticateInternalRequest
            {
                UserId = firebaseResponse.UserId
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

            var account = await GetAsyn(result.UserId);

            if (account.Status == AccountConstants.ACCOUNT_IS_INACTIVE)
            {
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR04", new string[] { "Account", "inactive" }).Value,
                    IsSuccess = false
                };
            }

            var accountViewModel = _mapper.Map<AccountDetailViewModel>(account);
            var message = new MessageResult("OK04", new string[] { "Login" });
            return new AuthenticateResponse(accountViewModel, result.Token, message, true, result.ExpireDate);
        }

        public async Task<ResultResponse> RegisterAccountAsync(RegisterRequest model)
        {
            var check = await CheckUserExistUserIdAndEmailAsync(model.UserId, model.Email);
            if (!check.IsSuccess)
                return check;

            check = CheckValidRole(model.Role);
            if (!check.IsSuccess)
                return check;

            if (model.Role.Equals(AccountConstants.ROLE_IS_ADMIN))
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR07", new string[] { "role" }).Value,
                    IsSuccess = false,
                };
            }


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

        public async Task<ResultResponse> UpdateAccountAsync(string userId, UpdateAccountViewModel model)
        {
            var check = await CheckValidUserAsync(userId);
            if (!check.IsSuccess)
                return check;

            var account = await GetAsyn(userId);

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
            
            var externalRequest = new AuthenticateExternalRequest
            {
                IdToken = model.IdToken,
                Provider = model.Provider,
                Role = model.Role
            };

            if (model.Role != null && model.Role.Length != 0)
            {
                var check = CheckValidRole(model.Role);
                if (!check.IsSuccess)
                    return new AuthenticateResponse
                    {
                        Message = check.Message,
                        IsSuccess = false,
                    };
            }

            var result = await _accountAuthenService.LoginAccountByExternalAsync(externalRequest);
            if (!result.IsSuccess)
            {
                return new AuthenticateResponse
                {
                    Message = result.Message,
                    IsSuccess = false,
                };
            }

            if (result.IsNewAccount)
            {
                if(result.Image == null || result.Image.Length == 0)
                {
                    result.Image = AccountConstants.DEFAULT_IMAGE;
                }
                var account = new Account
                {
                    Name = result.Name,
                    UserId = result.UserId,
                    Role = model.Role,
                    Email = result.Email,
                    Image = result.Image,
                    Status = AccountConstants.ACCOUNT_IS_ACTIVE,
                };
                await CreateAsyn(account);
            }

            var acc = await GetAsyn(result.UserId);

            if (acc.Status == AccountConstants.ACCOUNT_IS_INACTIVE)
            {
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR04", new string[] { "Account", "inactive" }).Value,
                    IsSuccess = false
                };
            }

            var accountViewModel = _mapper.Map<AccountDetailViewModel>(acc);
            var message = new MessageResult("OK04" , new string[] { "Login" });
            return new AuthenticateResponse(accountViewModel, result.Token, message, true, result.ExpireDate);
        }

        public async Task<ResultResponse> CheckUserExistUserIdAndEmailAsync(string userId, string email)
        {
            var accountExistsWithId = await GetAsyn(userId);
            var accountExistsWithEmail = GetByEmail(email);

            if (accountExistsWithId != null || accountExistsWithEmail != null)
                return new ResultResponse
                {
                    Message = new MessageResult("BR03", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            return new ResultResponse
            {
                IsSuccess = true
            };
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
                Message = new MessageResult("BR07", new string[] { "role" }).Value,
                IsSuccess = false,
            };
        }

        public async Task<ResultResponse> ChangePasswordAsync(string userId, ChangePassRequest model)
        {
            var account = await GetAsyn(userId);
            if (account == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Account" }).Value,
                    IsSuccess = false
                };
            }

            if (account.Status == AccountConstants.ACCOUNT_IS_INACTIVE)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR04", new string[] { "Account", "inactive" }).Value,
                    IsSuccess = false
                };
            }

            var requestModel = new ChangePasswordRequest()
            {
                UserId = userId,
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword
            };
            var result = await _accountAuthenService.ChangePasswordAsync(requestModel);

            if (!result.IsSuccess)
            {
                return new ResultResponse
                {
                    Message = result.Message,
                    IsSuccess = false,
                };
            }

            return new ResultResponse
            {
                Message = new MessageResult("OK04", new string[] { "Change password" }).Value,
                IsSuccess = true
            };
        }

        public async Task<ResultResponse> CheckValidUserAsync(string userId)
        {
            var account = await GetAsyn(userId);
            if (account == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "Account" }).Value,
                    IsSuccess = false
                };
            }

            if (account.Status == AccountConstants.ACCOUNT_IS_INACTIVE)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR04", new string[] { "Account", "inactive" }).Value,
                    IsSuccess = false
                };
            }

            return new ResultResponse
            {
                IsSuccess = true
            };
        }
    }
}
