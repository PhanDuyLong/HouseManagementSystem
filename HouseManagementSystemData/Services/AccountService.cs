using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HMS.Authen.Authentication;
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
using Microsoft.Extensions.Options;
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
        Task<AccountDetailViewModel> UpdateAccountAsync(string userId, UpdateAccountViewModel model);
        string DeleteAccount(Account account);
    }
    public partial class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationAccount> _accountManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AccountService(DbContext dbContext, IAccountRepository repository, IMapper mapper
            , UserManager<ApplicationAccount> accountManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _accountManager = accountManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
            var account = await _accountManager.FindByEmailAsync(model.Email);
            var acc = GetByEmail(model.Email);

            if (account == null || acc == null)
            {
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR06", new string[] { "email" }).Value,
                    IsSuccess = false,
                };
            }

            var result = await _accountManager.CheckPasswordAsync(account, model.Password);

            if (!result)
                return new AuthenticateResponse
                {
                    Message = new MessageResult("BR07").Value,
                    IsSuccess = false,
                };

            var userId = CustomFirebaseAuth(model.Email, model.Password);

            if (userId.Length != 0)
            {
                if (!userId.Equals(acc.UserId) || !userId.Equals(account.UserName))
                {
                    return new AuthenticateResponse
                    {
                        Message = new MessageResult("BR06", new string[] { "email" }).Value,
                        IsSuccess = false,
                    };
                }
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


            var accountViewModel = GetByUserId(account.UserName);

            var message = new MessageResult("OK04");

            return new AuthenticateResponse(accountViewModel, new JwtSecurityTokenHandler().WriteToken(token), message, true , token.ValidTo);
        }

        public async Task<ResultResponse> RegisterAccountAsync(RegisterRequest model)
        {
            var check = await IsAccountExistsAsync(model.UserId, model.Email);

            if (check)
                return new ResultResponse
                {
                    Message = new MessageResult("BRO3", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            ApplicationAccount account = new ApplicationAccount()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserId,
                PhoneNumber = model.Phone
            };

            var acc = _mapper.Map<Account>(model);
            acc.Password = null;

            var result = await _accountManager.CreateAsync(account, model.Password);

            if (!result.Succeeded)
                return new ResultResponse
                {
                    Message = new MessageResult("BR03", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            await CreateAsyn(acc);
            if (Get(acc.UserId) == null)
                return new ResultResponse
                {
                    Message = new MessageResult("BR03", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            if (!await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_ADMIN))
                await _roleManager.CreateAsync(new IdentityRole(AccountConstants.ROLE_IS_ADMIN));
            if (!await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_OWNER))
                await _roleManager.CreateAsync(new IdentityRole(AccountConstants.ROLE_IS_OWNER));
            if (!await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_TENACT))
                await _roleManager.CreateAsync(new IdentityRole(AccountConstants.ROLE_IS_TENACT));

            if (model.Role.Equals(AccountConstants.ROLE_IS_OWNER))
            {
                if (await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_OWNER))
                {
                    await _accountManager.AddToRoleAsync(account, AccountConstants.ROLE_IS_OWNER);
                }
            }

            if (model.Role.Equals(AccountConstants.ROLE_IS_TENACT))
            {
                if (await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_TENACT))
                {
                    await _accountManager.AddToRoleAsync(account, AccountConstants.ROLE_IS_TENACT);
                }
            }


            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Account" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> RegisterAdminAccountAsync(RegisterRequest model)
        {
            var check = await IsAccountExistsAsync(model.UserId, model.Email);

            if (check)
                return new ResultResponse
                {
                    Message = new MessageResult("BRO3", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            ApplicationAccount account = new ApplicationAccount()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserId,
                PhoneNumber = model.Phone
            };
            var acc = _mapper.Map<Account>(model);
            acc.Password = null;

            var result = await _accountManager.CreateAsync(account, model.Password);

            if (!result.Succeeded)
                return new ResultResponse
                {
                    Message = new MessageResult("BR03", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            await CreateAsyn(acc);
            if (Get(acc.UserId) == null)
                return new ResultResponse
                {
                    Message = new MessageResult("BR03", new string[] { "Account" }).Value,
                    IsSuccess = false,
                };

            if (!await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_ADMIN))
                await _roleManager.CreateAsync(new IdentityRole(AccountConstants.ROLE_IS_ADMIN));
            if (!await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_OWNER))
                await _roleManager.CreateAsync(new IdentityRole(AccountConstants.ROLE_IS_OWNER));
            if (!await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_TENACT))
                await _roleManager.CreateAsync(new IdentityRole(AccountConstants.ROLE_IS_TENACT));

            if (await _roleManager.RoleExistsAsync(AccountConstants.ROLE_IS_ADMIN))
            {
                await _accountManager.AddToRoleAsync(account, AccountConstants.ROLE_IS_ADMIN);
            }

            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "Account" }).Value,
                IsSuccess = true,
            };
        }

        public async Task<AccountDetailViewModel> UpdateAccountAsync(string userId, UpdateAccountViewModel model)
        {
            var account = await GetAsyn(userId);
            var acc = await _accountManager.FindByNameAsync(userId);
            if (account == null || acc == null)
                return null;
            bool isUpdateAccount = false;
            bool isUpdateAcc = false;

            if (model.Name != null)
            {
                account.Name = model.Name;
                isUpdateAccount = true;
            }

            if (model.Phone != null)
            {
                acc.PhoneNumber = model.Phone;
                account.Phone = model.Phone;
                isUpdateAccount = true;
                isUpdateAcc = true;
            }

            if (model.Image != null)
            {
                account.Image = model.Image;
                isUpdateAccount = true;
            }

            if (isUpdateAcc)
            {
                var result = await _accountManager.UpdateAsync(acc);
                if (!result.Succeeded)
                    return null;
            }

            if (isUpdateAccount)
                Update(account);

            return GetByUserId(account.UserId);
        }

        public async Task<AuthenticateResponse> LoginAccountByGoogleAsync(AuthenticateGoogleRequest model)
        {
            var account = await _accountManager.FindByNameAsync(model.UserId);
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
            return new AuthenticateResponse(accountViewModel, new JwtSecurityTokenHandler().WriteToken(token), message, true, token.ValidTo);
        }

        public async Task<bool> IsAccountExistsAsync(string userId, string email)
        {
            var accountExists = await _accountManager.FindByNameAsync(userId);
            var accountExistsWithEmail = await _accountManager.FindByEmailAsync(email);
            var accExists = GetByUserId(userId);
            var accExistsWithEmail = GetByEmail(email);

            if (accountExists != null || accExists != null || accExistsWithEmail != null || accountExistsWithEmail != null)
                return true;
            return false;
        }

        public string CustomFirebaseAuth(string email, string password)
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
                if (sResponseFromServer.Length != 0)
                {
                    var json = JObject.Parse(sResponseFromServer); ;
                    string userId = (string)json.SelectToken("localId");
                    return userId;
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                    return "";
            }
            return "";
        }

        public async Task<string> GoogleFirebaseAuthAsync(string userId)
        {
/*            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
            });
*/


            string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userId);
            return customToken;
        }
    }
}
