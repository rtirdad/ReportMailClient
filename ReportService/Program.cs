using GemBox.Document;
using System.Text.Json;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/report", (JsonDocument doc, HttpContext context) =>
{
    ComponentInfo.SetLicense("FREE-LIMITED-KEY");

    var templateDirectory = @".\Templates";
    var docRoot = doc.RootElement;

    var docFormat = docRoot.GetProperty("format").ToString();
    var templateName = docRoot.GetProperty("template").ToString();
    var filePath = Path.Combine(templateDirectory, $"{templateName}.docx");
    DocumentModel document = DocumentModel.Load(filePath);

    var clientData = docRoot.GetProperty("data");

    var pattern = @"\{{.*?\}}";
    Regex rg = new Regex(pattern);
    var matches = rg.Matches(document.Content.ToString());

    foreach (Match match in matches)
    {
        var inWordSomewhere = match.Value;

        var parts = inWordSomewhere.Split(new string[] { "{{", "}}", "." }, StringSplitOptions.RemoveEmptyEntries);

        var current = clientData;
        foreach (var part in parts)
        {
            current = current.GetProperty(part);
        }
        Console.WriteLine(current.ToString());
        document.Content.Replace(inWordSomewhere, current.ToString());
    }

    document.Content.Replace(new Regex("{Date}", RegexOptions.IgnoreCase),
        DateTime.Today.ToLongDateString());

    SaveOptions saveOptions;

    if (docFormat == "pdf")
    {
        var memoryStream = SaveToStream(document, new PdfSaveOptions() { ImageDpi = 220 });
        return Results.File(memoryStream, "application/pdf", "report.pdf");
    }
    else if (docFormat == "html")
    {
        saveOptions = new HtmlSaveOptions();
        var memoryStream = SaveToStream(document, saveOptions);
        return Results.File(memoryStream, "text/html", "report.html");
    }
    else
    {
        return Results.BadRequest("the format that you have provided is not supported, try pdf or html.");
    }
    Stream SaveToStream(DocumentModel document, SaveOptions saveOptions)
    {
        var memoryStream = new MemoryStream();
        memoryStream.Position = 0;

        document.Save(memoryStream, saveOptions);
        return memoryStream;
    }


})
.WithName("Report")
.WithOpenApi();

app.Run();