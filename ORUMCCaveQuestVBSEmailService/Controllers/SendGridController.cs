using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using SendGrid;

namespace ORUMCCaveQuestVBSEmailService.Controllers
{
    public class SendGridController : ApiController
    {
        private string _childName;
        private string _bccEmailAddress;
        private string _fromEmailAddress;
        private string _emailSubject;
        private Web _transportWeb { get; set; }

        public void SendEmail(string parentDestEmailAddress, string childName, string childUniqueId)
        {
            Trace.TraceInformation("Incoming Parent Email Address: " + parentDestEmailAddress);
            Trace.TraceInformation("Incoming Child Name: " + childName);
            Trace.TraceInformation("Incoming Storage URL: " + childUniqueId);
            _childName = childName;
            InitializeTransport();
            GetCommonEmailSettings();
            CreateParentEmail(parentDestEmailAddress);
            CreateDirectorEmail(childUniqueId);
        }

        private void InitializeTransport()
        {
            var APIKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            _transportWeb = new Web(APIKey);
        }

        private void GetCommonEmailSettings()
        {
            _bccEmailAddress = ConfigurationManager.AppSettings["BccEmailAddress"];
            _fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            _emailSubject = ConfigurationManager.AppSettings["EmailSubject"];
        }

        private void CreateParentEmail(string parentDestEmailAddress)
        {
            var email = new SendGridMessage();
            email.AddTo(parentDestEmailAddress);
            email.AddBcc(_bccEmailAddress);
            email.From = new MailAddress(_fromEmailAddress);
            email.Subject = _emailSubject + _childName;
            email.Text =
                @"We have received your registration.  We look forward to seeing you on Sunday, June 19th at 6:15pm!  
Have a blessed day!
 
The ORUMC Cave Quest VBS Team";
            DeliverMail(email);
        }

        private void CreateDirectorEmail(string childUniqueId)
        {
            var email = new SendGridMessage();
            email.AddTo(ConfigurationManager.AppSettings["DirectorEmailAddress"]);
            email.AddTo(ConfigurationManager.AppSettings["BackupEmailAddress"]);
            email.AddBcc(_bccEmailAddress);
            email.From = new MailAddress(_fromEmailAddress);
            email.Subject = _emailSubject + _childName;
            var childUniqueURL = ConfigurationManager.AppSettings["PersistentStorageURL"] + childUniqueId;
            var emailBody = @"A new child has been registered for this year's VBS.  Use this link if you would like to view his or her information:
{0}";
            email.Text = string.Format(emailBody, childUniqueURL);
            DeliverMail(email);
        }

        private void DeliverMail(SendGridMessage email)
        {
            try
            {
                _transportWeb.DeliverAsync(email);
                Trace.TraceInformation(email.ToString());
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.Message);
                Trace.TraceError(exception.StackTrace);
            }
        }
    }
}
