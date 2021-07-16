using HMS.Authen.Models;
using HMS.Authen.Utilities;
using HMS.Data.Requests;
using HMS.Data.Responses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Authen.Services
{
    public partial interface IAccountAuthenService
    {
        Task<AccountManagerResponse> LoginAccountAsync(AuthenticateInternalRequest request);
        Task<AccountManagerResponse> LoginAccountByExternalAsync(AuthenticateExternalRequest model);
        Task<AccountManagerResponse> RegisterAccountAsync(RegisterAccountRequest model);
    }
    public partial class AccountAuthenService : IAccountAuthenService
    {
        private readonly UserManager<ApplicationAccount> _accountManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtHandler _jwtHandler;

        public AccountAuthenService(UserManager<ApplicationAccount> accountManager, RoleManager<IdentityRole> roleManager, JwtHandler jwtHandler)
        {
            _accountManager = accountManager;
            _roleManager = roleManager;
            _jwtHandler = jwtHandler;
        }

        public async Task<AccountManagerResponse> LoginAccountAsync(AuthenticateInternalRequest model)
        {
            var user = await _accountManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return new AccountManagerResponse
                {
                    Message = "There is no user with that email address!",
                    IsSuccess = false,
                };
            }

            var result = await _accountManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new AccountManagerResponse
                {
                    Message = "Invalid password!",
                    IsSuccess = false
                };
            }

            var token = await _jwtHandler.GenerateToken(user);

            return new AccountManagerResponse
            {
                Token = token[0],
                Message = "Login succesfully",
                IsSuccess = true,
                ExpireDate = DateTime.Parse(token[1]),
                UserId = user.UserName
            };
        }

        public async Task<AccountManagerResponse> RegisterAccountAsync(RegisterAccountRequest model)
        {
            var accountExsitsByUserId = await _accountManager.FindByNameAsync(model.UserId);
            var accountExistsByEmail = await _accountManager.FindByEmailAsync(model.Email);

            if (accountExistsByEmail != null || accountExsitsByUserId != null)
                return new AccountManagerResponse
                {
                    Message = "Account already exists!",
                    IsSuccess = false,
                };

            ApplicationAccount account = new ApplicationAccount()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserId,
            };

            var result = await _accountManager.CreateAsync(account, model.Password);

            if (!result.Succeeded)
                return new AccountManagerResponse
                {
                    Message = "Account creation failed! Please check details and try again!",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _accountManager.AddToRoleAsync(account, model.Role);
            }

            return new AccountManagerResponse
            {
                Message = "Account created successfully",
                IsSuccess = true,
            };
        }

        public async Task<AccountManagerResponse> LoginAccountByExternalAsync(AuthenticateExternalRequest model)
        {
            var payload = _jwtHandler.PayloadInfo(model.IdToken);

            if (payload is null)
            {
                return new AccountManagerResponse
                {
                    Message = "Invalid external authentication.",
                    IsSuccess = false
                };
            }

            var info = new UserLoginInfo(model.Provider, payload.Sub, model.Provider);

            var user = await _accountManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user is null)
            {
                user = await _accountManager.FindByEmailAsync(payload["email"].ToString());

                if (user is null)
                {
                    if(model.Role.Length == 0)
                    {
                        return new AccountManagerResponse
                        {
                            Message = "New account need to role to create!",
                            IsSuccess = false
                        };
                    }

                    user = new ApplicationAccount()
                    {
                        Email = payload["email"].ToString(),
                        UserName = payload["email"].ToString()
                    };

                    var result = await _accountManager.CreateAsync(user);

                    if (!await _roleManager.RoleExistsAsync(model.Role))
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    if (await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _accountManager.AddToRoleAsync(user, model.Role);
                    }

                    await _accountManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        var token = await _jwtHandler.GenerateToken(user);
                        return new AccountManagerResponse
                        {
                            Token = token[0],
                            UserId = user.UserName,
                            Name = payload["name"].ToString(),
                            Email = user.Email,
                            Message = "Login successfully",
                            IsSuccess = true,
                            ExpireDate = DateTime.Parse(token[1]),
                            IsNewAccount = true
                        };
                    }
                }
            }
            else
            {
                await _accountManager.AddLoginAsync(user, info);
                var token = await _jwtHandler.GenerateToken(user);
                return new AccountManagerResponse
                {
                    UserId = user.UserName,
                    Email = user.Email,
                    Token = token[0],
                    Message = "Login successfully",
                    IsSuccess = true,
                    ExpireDate = DateTime.Parse(token[1]),
                    IsNewAccount = false
                };
            }

            return new AccountManagerResponse
            {
                Message = "Invalid external authentication.",
                IsSuccess = false
            };
        }
    }
}

