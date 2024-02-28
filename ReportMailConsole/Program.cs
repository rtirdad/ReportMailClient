using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var reportClient = new HttpClient();
var JsonData = @"
{
  ""format"": ""html"",
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

/*string reportFileName = "report.html";
if (reportResult.IsSuccessStatusCode)
{
    if (JsonData.Contains("\"format\": \"pdf\""))
    {
        FileName = "report.pdf";
    }
}*/

string reportFileName = "report.html"; // Default file name

if (reportResult.IsSuccessStatusCode)
{
    // Check if the format is "html" in JsonData
    if (JsonData.Contains("\"format\": \"html\""))
    {
        reportFileName = "report.html"; // Change the file name to "report.html"
    }
}

var emailClient = new HttpClient();

byte[] reportBytes;
using (var stream = await reportResult.Content.ReadAsStreamAsync())
using (var memoryStream = new MemoryStream())
{
    await stream.CopyToAsync(memoryStream);
    reportBytes = memoryStream.ToArray();
}
var encodedReport = Convert.ToBase64String(reportBytes);

var emailContent = new
{
    ToEmail = "ronat20003@gmail.com",
    ToDisplayName = "Rona",
    FromDisplayName = "R",
    FromMail = "ronat20003@gmail.com",
    Subject = "Console App Email with Report attachment",
    Body = "Email with attachment",
    Attachment = new[] { encodedReport },
    Format = JsonData.Contains("\"format\": \"pdf\"") ? "pdf" : "html"
};

//byte[] bytes = Encoding.Default.GetBytes(encodedReport);
var emailResult = await emailClient.PostAsJsonAsync("https://localhost:7154/Email/Send", emailContent);
if (emailResult.IsSuccessStatusCode && emailContent.Attachment != null)
{
    Console.WriteLine("Email sent successfully! :)");
}
else
{
    Console.WriteLine($"There was a problem with sending the email. :( ({emailResult.StatusCode})");
}

