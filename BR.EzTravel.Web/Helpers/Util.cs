using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BR.EzTravel.Web.Models;

namespace BR.EzTravel.Web.Helpers
{
    public static class Util
    {
        #region Session Variables

        public static tblmember SessionAccess
        {
            get
            {
                return HttpContext.Current.Session["Member"] as tblmember;
            }
            set
            {
                HttpContext.Current.Session["Member"] = value;
            }
        }

        public static string SessionErrMsg
        {
            get
            {
                return HttpContext.Current.Session["ErrMsg"] as string;
            }
            set
            {
                HttpContext.Current.Session["ErrMsg"] = value;
            }
        }

        public static void SetSessionErrMsg(Exception ex)
        {
            string errorMessage = "";

            errorMessage = string.Format("Exception occur. Error Message: {0}", ex.Message);

            Util.SessionErrMsg = errorMessage;
        }

        #endregion

        public static void SetSessionAccess(string loginID)
        {
            using (var context = new ExHolidayEntities())
            {
                var access = context.tblmembers.Single(a => a.PICEmail.ToLower() == loginID.ToLower() && a.Active);
                Util.SessionAccess = access;
            }
        }

        public static void CheckSessionAccess(RequestContext reqCtx)
        {
            if (Util.SessionAccess != null) return;

            var request = reqCtx.HttpContext.Request;
            var currentContenct = reqCtx.HttpContext;
            if (request.IsAuthenticated)
            {
                SetSessionAccess(currentContenct.User.Identity.Name);
            }
        }

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

        public static string GetMD5Hash(string value)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static int CalculateAverageRating(int sumRate, int totalReviews)
        {
            return totalReviews == 0 ? 0 : (int)Math.Round((double)sumRate / totalReviews, MidpointRounding.AwayFromZero);
        }

        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(8)]).ToArray());
        }

        public static int ConvertPriceSearch(string price)
        {
            return price.IsStringEmpty() ? 0 : int.Parse(price.Replace("$", ""));
        }
    }
}
