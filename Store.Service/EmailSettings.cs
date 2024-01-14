using Microsoft.Extensions.Options;
using MimeKit;
using Store.Core.Entities;
using Store.Core.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Mail;
namespace Store.Service
{
    public class EmailSettings : IEmailSettings
    {
        private readonly MailSettings options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            this.options = options.Value;
        }
        public void SendEmail(Mail email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(options.Email),
                Subject = email.Subject,
            };

            mail.To.Add(MailboxAddress.Parse(email.To));
            var builder = new BodyBuilder
            {
                HtmlBody = email.Body
            };
            mail.Body = builder.ToMessageBody();
            mail.From.Add(new MailboxAddress(options.DisplayName, options.Email));
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Timeout = 999999999;
            smtp.Connect(options.Host, options.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(options.Email, options.Password);
            smtp.Send(mail);
            smtp.Disconnect(true);

        }
    }
}
