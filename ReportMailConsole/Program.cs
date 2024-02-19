using GemBox.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

//generate report
var reportClient = new HttpClient();
var reportJsonContent = new
{
   Format: "pdf",
   template:"Letter1",
   data: 
    {
    client: {
        FirstName: "John",
        LastName: "Smith",
        PostCode: "1234AB",
        CompanyName: "Apple",
        Address: "Apple Street 123",
        Title: "Mr."
      }
}

var reportResult = await reportClient.PostAsJsonAsync("https://localhost:7251/report", reportJsonContent)


//Email can be sent succesfully 
var emailClient = new HttpClient();
var content = new
{
    toEmail = "Ronat20003@gmail.com",
    toDisplayName = "Rona",
    fromDisplayName = "R",
    fromMail = "ronat20003@gmail.com",
    subject = "console app email",
    body = "Hiii"
};

var result = await emailClient.PostAsJsonAsync("https://localhost:7154/Email/Send", content);

if (result.IsSuccessStatusCode)
{
    Console.WriteLine("Email sent successfully!");
}
else
{
    Console.WriteLine("Failed to send email.");
}
    