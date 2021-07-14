using HMS.Data.Utilities;
using HMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Responses
{
    public class AuthenticateResponse : ResultResponse
    {
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string Role { get; set; }


        public AuthenticateResponse() { }

        public AuthenticateResponse(AccountDetailViewModel accountViewModel, string token, MessageResult message, bool isSuccess, DateTime expireDate)
        {
            Name = accountViewModel.Name;
            UserId = accountViewModel.UserId;
            Phone = accountViewModel.Phone;
            Email = accountViewModel.Email;
            Image = accountViewModel.Image;
            Role = accountViewModel.Role;
            Token = token;
            IsSuccess = isSuccess;
            Message = message.Value;
            ExpireDate = expireDate;
        }
    }
}
