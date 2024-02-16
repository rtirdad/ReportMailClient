using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using MailService.Models;
using MailService.Settings;
using MailService.Models;
using MailService.Settings;
using MailService.Services;

namespace MailService.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> options)
        {
            _mailSettings = options.Value;
        }

        public async Task SendEmailAsync(MailRequest mailrequest)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(mailrequest.FromDisplayName, mailrequest.FromMail));
            //email.Sender = MailboxAddress.Parse(mailrequest.FromAppPassword);
            email.To.Add(new MailboxAddress(mailrequest.ToDisplayName, mailrequest.ToEmail));

            email.Subject = mailrequest.Subject;
            var builder = new BodyBuilder();

            builder.HtmlBody = mailrequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
