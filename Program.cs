using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Низа од 26 букви од латинската азбука (индекси од 0 до 25)
char[] alphabet = new char[] 
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
};

app.MapGet("/", async (HttpContext context) =>
{
    var html = @"
    <!DOCTYPE html>
    <html lang='mk'>
    <head>
        <meta charset='UTF-8'>
        <title>Калкулатор за Индекс на Букви</title>
        <style>
            body { font-family: Arial, sans-serif; margin: 40px; background-color: #f8f9fa; }
            .card { background: white; padding: 25px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1); max-width: 450px; }
            .form-group { margin-bottom: 20px; }
            label { display: block; margin-bottom: 8px; font-weight: bold; color: #333; }
            input[type='number'] { width: 100%; padding: 10px; box-sizing: border-box; border: 1px solid #ccc; border-radius: 4px; font-size: 16px; }
            button { width: 100%; background-color: #0d6efd; color: white; padding: 10px; border: none; font-size: 16px; border-radius: 4px; cursor: pointer; }
            button:hover { background-color: #0b5ed7; }
        </style>
    </head>
    <body>
        <div class='card'>
            <h2>Проверка на Буква од Азбука</h2>
            <form action='/process' method='POST'>
                <div class='form-group'>
                    <label for='numberInput'>Внесете реден број (1 - 26):</label>
                    <input type='number' id='numberInput' name='numberInput' required />
                </div>
                
                <button type='submit'>Пресметај</button>
            </form>
        </div>
    </body>
    </html>";

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

app.MapPost("/process", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    string rawInput = form["numberInput"];

    // Директна парсирање и конверзија БЕЗ проверка на валидност
    int position = int.Parse(rawInput);

    // 1. Прикажување на соодветната буква за внесениот реден број (1-26)
    // Бидејќи масивот користи 0-базиран индекс (0 до 25), одземаме 1
    char inputLetter = alphabet[position - 1];

    // 2. Пресметка: реден број поделен со 3 со Floor заокружување
    int dividedPosition = (int)Math.Floor((double)position / 3);

    // Земање на соодветната буква за поделениот реден број
    char calculatedLetter = alphabet[dividedPosition - 1];

    string responseHtml = $@"
    <!DOCTYPE html>
    <html lang='mk'>
    <head>
        <meta charset='UTF-8'>
        <title>Резултат</title>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 40px; background-color: #f8f9fa; }}
            .card {{ background: white; padding: 25px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1); max-width: 450px; }}
            .result {{ font-size: 18px; margin-top: 15px; color: #212529; }}
            .highlight {{ font-weight: bold; color: #0d6efd; }}
            a {{ display: inline-block; margin-top: 15px; color: #0d6efd; text-decoration: none; }}
        </style>
    </head>
    <body>
        <div class='card'>
            <h2>Резултат:</h2>
            <div class='result'>
                <p>Избрана буква за бројот {position}: <span id = 'mainresult' class='highlight'>{inputLetter}</span></p>
                <p>Буквата со реден број поделен со 3 е: <span id = 'otherresult' class='highlight'>{calculatedLetter}</span></p>
            </div>
            <a href='/'>← Назад</a>
        </div>
    </body>
    </html>";

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(responseHtml);
});

app.Run();