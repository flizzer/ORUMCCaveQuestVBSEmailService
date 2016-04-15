using System;
using System.Collections.Generic;
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
        public void SendEmail(string destEmailAddress, string childName)
        {
            var APIKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var transportWeb = new Web(APIKey);
            var email = new SendGridMessage();
            email.AddTo(destEmailAddress);
            email.AddBcc("briandavis1977@gmail.com");
            email.From = new MailAddress("donotreply@oakridgeumc.org");
            email.Subject = "CaveQuest VBS Registration for " + childName;
            email.Text = 
@"We have received your registration.  We look forward to seeing you on Sunday, June 19th at 6:15pm!  
Have a blessed day!
 
- The ORUMC Cave Quest VBS Team";
            transportWeb.DeliverAsync(email);
        }
    }
}
