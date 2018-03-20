using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WarehouseManagementSystem.Models
{
    public class SendGridModel
    {
        public static string GetAPIKey()
        {
            return System.Configuration.ConfigurationManager.AppSettings[""].ToString();
        }
        public class Message
        {
            public string From { get; set; }
            public List<string> To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public dynamic Attachments { get; set; }

            public void Send()
            {

            }
        }
    }
}