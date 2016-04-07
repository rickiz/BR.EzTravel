using BR.EzTravel.Web.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace BR.EzTravel.Web.Helpers
{
    public class EmailClient
    {
        private string _Host;
        private string _UserID;
        private string _Password;
        private int _Port;
        private string _From;
        private bool _IsEnableSsl;
        private bool _UseDefaultCredentials;

        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailClient()
        {
            _Host = Settings.Default.EmailHost;
            _Port = Settings.Default.EmailPort;
            _UserID = Settings.Default.EmailUsername;
            _Password = Settings.Default.EmailPassword;
            _From = Settings.Default.EmailFrom;
            _IsEnableSsl = Settings.Default.EmailIsEnabledSSL;
            _UseDefaultCredentials = Settings.Default.EmailDefaultCrudential;
        }

        public void SendEmail() { SendEmail(null); }

        public void SendEmail(string[] fileNames)
        {
            MailMessage mail = new MailMessage();
            //multiple recipient

            string[] emailAdds = To.Split(';');
            foreach (string emailAdd in emailAdds) { if (!string.IsNullOrEmpty(emailAdd)) mail.To.Add(emailAdd); }
            emailAdds = Cc.Split(';');
            foreach (string emailAdd in emailAdds) { if (!string.IsNullOrEmpty(emailAdd)) mail.CC.Add(emailAdd); }
            emailAdds = Bcc.Split(';');
            foreach (string emailAdd in emailAdds) { if (!string.IsNullOrEmpty(emailAdd)) mail.Bcc.Add(emailAdd); }

            //attach excel file
            if (fileNames != null)
            {
                foreach (string fileName in fileNames)
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Attachment attachFile = new Attachment(fileName);
                        mail.Attachments.Add(attachFile);
                    }
                }
            }

            mail.From = new MailAddress(_From);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;

            SmtpClient SmtpMail = new SmtpClient();
            SmtpMail.Host = _Host;
            SmtpMail.Port = _Port;
            //SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpMail.EnableSsl = _IsEnableSsl;

            if (_UseDefaultCredentials)
                SmtpMail.UseDefaultCredentials = true;
            else
                SmtpMail.Credentials = new System.Net.NetworkCredential(_UserID, _Password);

            SmtpMail.Send(mail);
        }
    }
}