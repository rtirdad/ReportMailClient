//using GemBox.Document;
using GemBox.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


ComponentInfo.SetLicense("FREE-LIMITED-KEY");

var reportClient = new HttpClient();
var reportJsonContent = new
{
    format = "pdf",
    template = "Letter1",
    data = new
    {
        client = new
        {
            FirstName = "John",
            LastName = "Smith",
            PostCode = "1234AB",
            CompanyName = "Apple",
            Address = "Apple Street 123",
            Title = "Mr."
        }
    }
};
var reportResult = await reportClient.PostAsJsonAsync("https://localhost:7251/report", reportJsonContent);

string reportFilePath = Path.GetTempFileName();
using (var stream = await reportResult.Content.ReadAsStreamAsync())
using (var fileStream = File.Create(reportFilePath))
{
    await stream.CopyToAsync(fileStream);
}

var emailClient = new HttpClient();
var emailContent = new
{
    toEmail = "Ronat20003@gmail.com",
    toDisplayName = "Rona",
    fromDisplayName = "R",
    fromMail = "ronat20003@gmail.com",
    subject = "Console App Email with Report",
    body = "Hi there, please find attached the report.",
    attachmentPath = reportFilePath
};

var emailResult = await emailClient.PostAsJsonAsync("https://localhost:7154/Email/SendWithAttachment", emailContent);

if (emailResult.IsSuccessStatusCode)
{
    Console.WriteLine("Email sent successfully!");
}
else
{
    Console.WriteLine("Failed to send email.");
}