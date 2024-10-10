using LilamiBazzar.Models.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Utility.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(Email request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["SMTPConnectionStrings:EmailUserName"]));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["SMTPConnectionStrings:EmailHost"], 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["SMTPConnectionStrings:EmailUserName"], _configuration["SMTPConnectionStrings:EmailPassword"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
