using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using static System.Collections.Specialized.NameObjectCollectionBase;
using System.Collections.Specialized;

namespace WarehouseManagementSystem.Models
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            string APIKey = System.Configuration.ConfigurationManager.AppSettings[""].ToString();
            SendGridClient client = new SendGridClient(APIKey);
            EmailAddress from = new EmailAddress("");
            EmailAddress to = new EmailAddress(message.Destination);
            SendGridMessage email = MailHelper.CreateSingleEmail(from, to, message.Subject, message.Body, message.Body);

            await client.SendEmailAsync(email);
        }


    }
}