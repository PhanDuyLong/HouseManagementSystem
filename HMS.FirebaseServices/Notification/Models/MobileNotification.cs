using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.FirebaseNotification
{
    public class MobileNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}