using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MailService.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string ToDisplayName { get; set; }
        public string FromDisplayName { get; set; }

        public string FromMail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string[] Attachment { get; set; }

    }
}

