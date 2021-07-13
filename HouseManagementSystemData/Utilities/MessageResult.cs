using HMS.Data.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Utilities
{
    public class MessageResult
    {
        public string MessageCode;
        public string[] Parameters;
        public string Value;

        public MessageResult(string messageCode, string[] paramters)
        {
            MessageCode = messageCode;
            Parameters = paramters;
            Value = string.Format(typeof(MessageConstants).GetField(messageCode).GetValue(0).ToString(), paramters);
        }

        public MessageResult(string messageCode)
        {
            MessageCode = messageCode;
            Value = typeof(MessageConstants).GetField(messageCode).GetValue(0).ToString();
        }
    }
}
