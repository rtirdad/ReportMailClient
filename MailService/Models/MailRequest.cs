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

        public string AttachmentPath { get; set; }

        //public IFormFileCollection? Attachments { get; set; }
        
        //public string JsonData { get; set; }

        //public JsonElement GetJsonDataAsJsonElement() => JsonDocument.Parse(JsonData).RootElement;
    }
}
