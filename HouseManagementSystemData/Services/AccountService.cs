using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Requests;
using HMS.Data.Responses;
using HMS.Data.Services.Base;
using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace HMS.Data.Services
{
    public partial interface IAccountService : IBaseService<Account>
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<AccountBaseViewModel> GetAll();
        AccountBaseViewModel GetByUsername(string username);
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
            var account = Get().Where(a => a.Username == model.Username && a.Password == model.Password).FirstOrDefault();

            // return null if user not found
            if (account == null) return null;

            var accountViewModel = _mapper.Map<AccountBaseViewModel>(account);

            // authentication successful so generate jwt token
            var token = generateJwtToken(accountViewModel);

            return new AuthenticateResponse(accountViewModel, token);
        }

        public IEnumerable<AccountBaseViewModel> GetAll()
        {
            return Get().ProjectTo<AccountBaseViewModel>(_mapper.ConfigurationProvider);
        }

        public AccountBaseViewModel GetByUsername(string username)
        {
            var account = Get().Where(a => a.Username == username).FirstOrDefault();

            var accountViewModel = _mapper.Map<AccountBaseViewModel>(account);

            return accountViewModel;
        }

        private string generateJwtToken(AccountBaseViewModel accountViewModel)
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
