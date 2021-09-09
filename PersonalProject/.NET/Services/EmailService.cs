using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sabio.Models.AppSettings;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Sabio.Services
{
    public class EmailService
    {
        //private AppKeys _appKeys;
        //public EmailService(IOptions<AppKeys> appKeys)
        //{
        //    _appKeys = appKeys.Value; //keeps saying it null
        //}
        public async Task SendEmail(string appKeys, SendGridMessage msg)
        {
            string apiKey = appKeys;
            var client = new SendGridClient(apiKey);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task VerifyEmail(string appKeys, string email, string fName, string token)
        {
            SendGridMessage msg = new SendGridMessage();

            msg.From = new EmailAddress(email);
            msg.Subject = "Email Verification";
            //var to = new EmailAddress(email, fName);
            var to = new EmailAddress("amytest@mailinator.com");
            msg.PlainTextContent = "Please click the link  to verify your email";
            msg.HtmlContent = $"Please click <a href='http://localhost:3000/confirmation/{token}'>here</a> to verify your email";
            msg.AddTo(to);

            await SendEmail(appKeys, msg);
        }

        public async Task SendResetPassword(string appKeys, string email, string token)
        {
            SendGridMessage msg = new SendGridMessage();

            msg.From = new EmailAddress(email);
            msg.Subject = "Password Reset";
            //var to = new EmailAddress(email, fName);
            var to = new EmailAddress("amytest@mailinator.com");
            msg.PlainTextContent = "Click link to reset password";
            msg.HtmlContent = $"Please click <a href='http://localhost:3000/passwordReset/{token}'>here</a> to verify your email";
            msg.AddTo(to);

            await SendEmail(appKeys, msg);
        }

    }
}
