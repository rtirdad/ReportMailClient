﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var reportClient = new HttpClient();
var JsonData = @"
{
  ""format"": ""pdf"",
  ""template"": ""Letter1"",
  ""data"": 
  { 
    ""client"": {
      ""FirstName"": ""John"",
      ""LastName"": ""Smith"",
      ""PostCode"": ""1234AB"",
      ""CompanyName"": ""Apple"",
      ""Address"": ""Apple Street 123"",
      ""Title"": ""Mr.""
    }
  }
}";
var reportJsonContent = JsonDocument.Parse(JsonData);

var reportResult = await reportClient.PostAsJsonAsync("https://localhost:7251/report", reportJsonContent);

string reportFileName = "report.html";
if (reportResult.IsSuccessStatusCode)
{
    if (JsonData.Contains("\"format\": \"pdf\""))
    {
        reportFileName = "report.pdf";
    }
}

byte[] reportBytes;
using (var stream = await reportResult.Content.ReadAsStreamAsync())
using (var memoryStream = new MemoryStream())
{
    await stream.CopyToAsync(memoryStream);
    reportBytes = memoryStream.ToArray();
}
/*string reportFilePath = Path.Combine(Path.GetTempPath(), reportFileName);
using (var stream = await reportResult.Content.ReadAsStreamAsync())
using (var fileStream = File.Create(reportFilePath))
{
    await stream.CopyToAsync(fileStream);
}*/

//string encodedReport = Encoding.Default.GetString(reportBytes);
var encodedReport = Convert.ToBase64String(reportBytes);


var emailClient = new HttpClient();

var emailContent = new
{
    ToEmail = "ronat20003@gmail.com",
    ToDisplayName = "Rona",
    FromDisplayName = "R",
    FromMail = "ronat20003@gmail.com",
    Subject = "Console App Email with Report attachment",
    Body = "Email with attachment",
    Attachment = encodedReport
    //Attachment = new[] { reportFilePath }
};

byte[] bytes = Encoding.Default.GetBytes(encodedReport);

var emailResult = await emailClient.PostAsJsonAsync("https://localhost:7154/Email/Send", emailContent);
if (emailResult.IsSuccessStatusCode && emailContent.Attachment != null)
{
    Console.WriteLine("Email sent successfully with an attachment! :)");
}
else
{
    Console.WriteLine($"There was a problem with sending the email. :( ({emailResult.StatusCode})");
}
