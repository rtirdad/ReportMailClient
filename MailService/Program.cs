using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailService.Models;
using MailService.Settings;
using System.Net.Mail;
using MimeKit.Text;
using MailKit.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MailSettings
//builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Register MailRequest
//builder.Services.AddSingleton<MailRequest>();
builder.Services.AddTransient<MailRequest>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/SendMail", async ( MailSettings mailSettings, MailRequest mailrequest) =>
{
    var email = new MimeMessage();
    email.From.Add(new MailboxAddress(mailrequest.FromDisplayName, mailrequest.FromMail));
    //email.Sender = MailboxAddress.Parse(mailrequest.FromAppPassword);
    email.To.Add(new MailboxAddress(mailrequest.ToDisplayName, mailrequest.ToEmail));

    email.Subject = mailrequest.Subject;
    var builder = new BodyBuilder();

    if (mailrequest.Attachments != null)
    {
        byte[] fileBytes;
        foreach (var file in mailrequest.Attachments)
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            }
        }
    }
    builder.HtmlBody = mailrequest.Body;
    email.Body = builder.ToMessageBody();
    using var smtp = new MailKit.Net.Smtp.SmtpClient();
    smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
    smtp.Authenticate(mailrequest.FromMail, mailSettings.Password);
    await smtp.SendAsync(email);
    smtp.Disconnect(true);

})
.WithName("SendMail")
.WithOpenApi();

app.Run();

