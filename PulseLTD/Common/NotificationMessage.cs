using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PulseLTD.Common
{
    public class NotificationMessage
    {
        public string Message { get; set; }
        public Enums.NotifyType Type { get; set; }

        public NotificationMessage(string msg, Enums.NotifyType errorType)
        {
            Message = msg;
            Type = errorType;
        }
    }
}