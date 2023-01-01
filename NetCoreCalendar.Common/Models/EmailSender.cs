using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace NetCoreCalendar.Models
{
    public class EmailSender : IEmailSender
    {
        /**
         * Creates a confirmation email that we will send to our users for them to
         * proceed with the registration process; uses a third party email to send the message
         **/
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("teacherplannerapp@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using(var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit
                    .Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("teacherplannerapp@gmail.com", "yjegohhglwvmgwzu");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }
            return Task.CompletedTask;
        }
    }
}
