using HMS.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Data.Responses
{
    public class ResultResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public ResultResponse() { }

        public ResultResponse(MessageResult message, bool isSuccess)
        {
            Message = message.Value;
            IsSuccess = isSuccess;
        }
    }
}
