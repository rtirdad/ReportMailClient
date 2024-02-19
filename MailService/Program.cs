using Microsoft.Extensions.Configuration;
using MailService.Services;
using MailService.Settings;
using System.Text.RegularExpressions;
using System.Text.Json;
using MailService.Models;
using MailService.Models;
using MailService.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<IMailService,MailService.Services.MailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapPost("Email/Send", async (IMailService mailService, MailRequest request) =>
{
    try
    {
        await mailService.SendEmailAsync(request);
        app.Logger.LogInformation("Email sent successfully :)");
        return Results.Ok();
    }
    catch (Exception ex)
    {
        app.Logger.LogError("An error occurred, email send unsuccessfully :(");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})

.WithName("Report")
.WithOpenApi();
app.Run();
