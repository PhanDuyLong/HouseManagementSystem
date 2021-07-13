using HMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Responses
{
    public class AuthenticateResponse : AccountDetailViewModel
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime? ExpireDate { get; set; }

        public AuthenticateResponse() { }

        public AuthenticateResponse(AccountDetailViewModel accountViewModel, string token, bool isSuccess, DateTime expireDate)
        {

            Name = accountViewModel.Name;
            UserId = accountViewModel.UserId;
            Phone = accountViewModel.Phone;
            Email = accountViewModel.Email;
            Role = accountViewModel.Role;
            Token = token;
            IsSuccess = isSuccess;
            ExpireDate = expireDate;
        }
    }
}
