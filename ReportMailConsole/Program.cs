using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Retrieve the report as an attachment
        //var reportClient = new HttpClient();
        /*var reportRequest = new
        {
            format = "Pdf",
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
        };*/
        /*var result = await reportClient.PostAsJsonAsync("https://localhost:7251/report", JsonContent.Create(@"{
          ""format"": ""pdf"",
          ""template"":""Letter1"",
          ""data"": 
            { 
              ""client"": {
                ""FirstName"": ""John"",
                ""LastName"": ""Smith"",
                ""PostCode"": ""1234AB"",
                ""CompanyName"": ""Apple"",
                ""Address"": ""Apple Street 123"",
                ""Title"": ""Mr.""
              }}
        }
        "));

        //var reportResponse = await reportClient.PostAsJsonAsync("https://localhost:7251/report", result);
        byte[] attachment = null;

        if (result.IsSuccessStatusCode)
        {
            attachment = await result.Content.ReadAsByteArrayAsync();
            Console.WriteLine("report has been generated");
        }
        else
        {
            Console.WriteLine($"Failed to generate report. Status code: {result.StatusCode}");
            return;
        }*/

        // 2. Send email with the report attachment
        var emailClient = new HttpClient();
        var mailRequest = new
        {
            ToEmail = "ronat20003@gmail.com",
            ToDisplayName = "r",
            FromDisplayName = "Your Name",
            FromMail = "ronat20003@gmail.com",
            Subject = "Report Attached",
            Body = "Please find the attached report."
        };

        var formData = new MultipartFormDataContent();
        //formData.Add(new ByteArrayContent(attachment), "attachment", "report.pdf");
        formData.Add(JsonContent.Create(mailRequest), "mailRequest");

        var mailResponse = await emailClient.PostAsync("https://localhost:7154/Email/Send", formData);

        if (mailResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Email sent successfully!");
        }
        else
        {
            Console.WriteLine($"An error has occured when sending the email {mailResponse.StatusCode}");
        }
    }
}
