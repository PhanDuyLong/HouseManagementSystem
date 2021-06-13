using HMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Responses
{
    public class AuthenticateResponse
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(AccountBaseViewModel accountViewModel, string token)
        {

            Name = accountViewModel.Name;
            Username = accountViewModel.Username;
            Phone = accountViewModel.Phone;
            Email = accountViewModel.Email;
            Role = accountViewModel.Role;
            Token = token;
        }
    }
}
