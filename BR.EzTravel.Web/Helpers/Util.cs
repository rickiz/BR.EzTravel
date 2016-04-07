using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;

namespace BR.EzTravel.Web.Helpers
{
    public static class Util
    {
        public static void SendEmail(string subject, string body, string toEmails, string bccEmails, string ccEmails)
        {
            var emailClient = new Helpers.EmailClient
            {
                Bcc = bccEmails ?? "",
                Cc = ccEmails ?? "",
                To = toEmails,
                Subject = subject,
                Body = body
            };

            emailClient.SendEmail(new string[] { });
        }
    }
}
