using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Constants;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Requests;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IAccountService : IBaseService<Account>
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        AccountDetailViewModel GetByUsername(string username);
        List<AccountTenantViewModel> GetTenantNames();
        Task<string> CreateAccount(CreateAccountViewModel model);
        UpdateAccountViewModel UpdateAccount(Account account, UpdateAccountViewModel model);
        string UpdatePassword(Account account, string password);
        string DeleteAccount(Account account);
    }
    public partial class AccountService : BaseService<Account>, IAccountService
    {
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        public AccountService(DbContext dbContext, IAccountRepository repository, IMapper mapper, IOptions<AppSettings> appSettings) : base(dbContext, repository)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var account = Get().Where(a => a.Username == model.Username && a.Password == model.Password && a.Status == AccountConstants.STATUS_IS_ACTIVE).FirstOrDefault();

            // return null if user not found
            if (account == null) return null;

            var accountViewModel = _mapper.Map<AccountDetailViewModel>(account);

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(accountViewModel);

            return new AuthenticateResponse(accountViewModel, token);
        }

        public async Task<string> CreateAccount(CreateAccountViewModel model)
        {
            var account = _mapper.Map<Account>(model);
            await CreateAsyn(account);
            return "Created succesfully";
        }

        public string DeleteAccount(Account account)
        {
            account.Status = AccountConstants.STATUS_IS_INACTIVE;
            Update(account);
            return "Deleted succesfully";
        }

        public AccountDetailViewModel GetByUsername(string username)
        {
            var account = Get().Where(a => a.Username == username).FirstOrDefault();

            var accountViewModel = _mapper.Map<AccountDetailViewModel>(account);

            return accountViewModel;
        }

        public List<AccountTenantViewModel> GetTenantNames()
        {
            var tenants = Get().Where(a => a.Role == AccountConstants.ROLE_IS_TENACT).ProjectTo<AccountTenantViewModel>(_mapper.ConfigurationProvider).ToList();
            return tenants;
        }

        public UpdateAccountViewModel UpdateAccount(Account account, UpdateAccountViewModel model)
        {
            account.Email = model.Email;
            account.Name = model.Name;
            account.Phone = model.Phone;
            Update(account);
            return model;
        }

        public string UpdatePassword(Account account, string password)
        {
            account.Password = password;
            Update(account);
            return "Changed successfully";
        }

        private string GenerateJwtToken(AccountDetailViewModel accountViewModel)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("username", accountViewModel.Username.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
