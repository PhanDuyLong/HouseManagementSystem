using HMS.Authen.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Authen.Utilities
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;
        private readonly UserManager<ApplicationAccount> _accountManager;
        public JwtHandler(IConfiguration configuration, UserManager<ApplicationAccount> accountManager)
        {
            _accountManager = accountManager;
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JWT");
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("Secret").Value);

            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(ApplicationAccount account)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var roles = await _accountManager.GetRolesAsync(account);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var iss = _jwtSettings.GetSection("ValidIssuer").Value;
            var aud = _jwtSettings.GetSection("ValidAudience").Value;
            var timeExpireInStr = _jwtSettings.GetSection("ExpireInMonths").Value;
            var timeExpire = Int32.Parse(timeExpireInStr);

            var tokenOptions = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(timeExpire),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        public async Task<List<String>> GenerateToken(ApplicationAccount account)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(account);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            List<String> result = new List<string>
            {
                token,
                tokenOptions.ValidTo.ToString()
            };

            return result;
        }

        public JwtPayload PayloadInfo(string idToken)
        {
            var token = idToken;
            var handler = new JwtSecurityTokenHandler();
            var tokenData = handler.ReadJwtToken(token);
            return tokenData.Payload;
        }
    }
}
