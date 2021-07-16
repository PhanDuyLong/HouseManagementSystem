using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Responses
{
    public class AccountManagerResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsNewAccount { get; set; }

        public string GetErrors()
        {
            var tmp = "";
            foreach(string error in Errors)
            {
                tmp += error + "\n";
            }
            return tmp;
        }
    }
}
