using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
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

string reportFilePath = Path.Combine(Path.GetTempPath(), reportFileName);
using (var stream = await reportResult.Content.ReadAsStreamAsync())
using (var fileStream = File.Create(reportFilePath))
{
    await stream.CopyToAsync(fileStream);
}

var emailClient = new HttpClient();
/*var JsonEmailData = @"
{
  ""toEmail"": ""ronat20003@gmail.com"",
  ""toDisplayName"": ""Rona"",
  ""fromDisplayName"": ""R"",
  ""fromMail"": ""ronat20003@gmail.com"",
  ""subject"": ""Test to see if email sends"",
  ""body"": ""Hiii"",
  ""attachmentPath"": """ + reportFilePath + @"""
}";*/

var emailContent = new
{
    toEmail = "ronat20003@gmail.com",
    toDisplayName = "Rona",
    fromDisplayName = "R",
    fromMail = "ronat20003@gmail.com",
    subject = "Console App Email with Report attachment",
    body = "Email with attachment",
    attachmentPath = reportFilePath
};
//var emailContent = JsonDocument.Parse(JsonEmailData);

var emailResult = await emailClient.PostAsJsonAsync("https://localhost:7154/Email/Send", emailContent);
if (emailResult.IsSuccessStatusCode)
{
    Console.WriteLine("Email sent successfully!");
}
else
{
    Console.WriteLine("Failed to send email.");
}

