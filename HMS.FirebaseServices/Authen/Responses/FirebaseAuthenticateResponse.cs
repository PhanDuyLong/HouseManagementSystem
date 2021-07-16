using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.FirebaseServices.Authen.Responses
{
    public class FirebaseAuthenticateResponse
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
