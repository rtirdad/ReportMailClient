using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
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

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(mailRequest.FromDisplayName, mailRequest.FromMail));
            email.To.Add(new MailboxAddress(mailRequest.ToDisplayName, mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;

            if (mailRequest.Attachment != null)
            {
                foreach (var attachmentData in mailRequest.Attachment)
                {
                    if (!string.IsNullOrEmpty(attachmentData))
                    {
                        var attachmentBytes = Convert.FromBase64String(attachmentData);
                        var attachmentContent = new MimePart("application", "pdf")
                        {
                            Content = new MimeContent(new MemoryStream(attachmentBytes), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = mailRequest.Format == "pdf" ? "report.pdf" : "report.html"
                        };
                        builder.Attachments.Add(attachmentContent);
                    }
                }
            }
            email.Body = builder.ToMessageBody();

            //Mail account/port can be changed in appsettings.json
            //password needs to be an app password in the case of a gmail account, account password will throw errors 
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}


