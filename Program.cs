using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Овозможување сервирање на статички фајлови (CSS/JS од wwwroot)
app.UseStaticFiles();

char[] alphabet = new char[] 
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 
    'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 
    'U', 'V', 'W', 'X', 'Y', 'Z'
};

app.MapGet("/", async (HttpContext context) =>
{
    int randomPosition = Random.Shared.Next(3, 27);
    char givenLetter = alphabet[randomPosition - 1];

    string template = await File.ReadAllTextAsync("Views/Index.html");
    string html = template
        .Replace("{givenLetter}", givenLetter.ToString())
        .Replace("{randomPosition}", randomPosition.ToString());

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

app.MapPost("/process", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    string rawPosition = form["randomPosition"];
    string rawInput = form["letterInput"].ToString().Trim();

    int position = int.Parse(rawPosition);
    char givenLetter = alphabet[position - 1];

    if (string.IsNullOrEmpty(rawInput) || rawInput.Length != 1 || !char.IsLetter(rawInput[0]))
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync("<h2>Невалиден внес! Молиме внесете само една буква.</h2><a href='/'>← Назад</a>");
        return;
    }

    char userLetter = char.ToUpper(rawInput[0]);
    int expectedPosition = (int)Math.Floor((double)position / 3);
    char expectedLetter = alphabet[expectedPosition - 1];

    bool isCorrect = (userLetter == expectedLetter);
    string nextButtonHtml = isCorrect 
        ? "<a href='/reservation' class='btn-next'>Продолжи кон резервација →</a>" 
        : "";

    string template = await File.ReadAllTextAsync("Views/Process.html");
    string html = template
        .Replace("{givenLetter}", givenLetter.ToString())
        .Replace("{position}", position.ToString())
        .Replace("{expectedLetter}", expectedLetter.ToString())
        .Replace("{userLetter}", userLetter.ToString())
        .Replace("{nextButtonHtml}", nextButtonHtml);

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

// Мапирање на издвоените модули
ReservationEndpoints.MapReservationEndpoints(app);
QueueEndpoints.MapQueueEndpoints(app);

app.Run();