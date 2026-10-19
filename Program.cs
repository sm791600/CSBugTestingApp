using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Низа од 26 букви од латинската азбука (индекси од 0 до 25)
char[] alphabet = new char[] 
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 
    'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 
    'U', 'V', 'W', 'X', 'Y', 'Z'
};

app.MapGet("/", async (HttpContext context) =>
{
    // Генерирање случаен реден број помеѓу 3 и 26 (вклучително)
    int randomPosition = Random.Shared.Next(3, 27);
    char givenLetter = alphabet[randomPosition - 1];

    var html = $@"
    <!DOCTYPE html>
    <html lang='mk'>
    <head>
        <meta charset='UTF-8'>
        <title>Калкулатор за Индекс на Букви</title>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 40px; background-color: #f8f9fa; }}
            .card {{ background: white; padding: 25px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1); max-width: 450px; }}
            .form-group {{ margin-bottom: 20px; }}
            label {{ display: block; margin-bottom: 8px; font-weight: bold; color: #333; }}
            input[type='text'] {{ width: 100%; padding: 10px; box-sizing: border-box; border: 1px solid #ccc; border-radius: 4px; font-size: 16px; }}
            button {{ width: 100%; background-color: #0d6efd; color: white; padding: 10px; border: none; font-size: 16px; border-radius: 4px; cursor: pointer; }}
            button:hover {{ background-color: #0b5ed7; }}
            .highlight {{ font-weight: bold; color: #0d6efd; }}
        </style>
    </head>
    <body>
        <div class='card'>
            <p>Погодете која буква е за 3 пати помала од редниот број на азбуката кој што е прикажан:</p>
            <h3>Прикажана буква: <span id='givenLetterDisplay' class='highlight'>{givenLetter}</span> (Реден број: <span id='randomPositionDisplay'>{randomPosition}</span>)</h3>
            
            <form action='/process' method='POST'>
                <input type='hidden' name='randomPosition' value='{randomPosition}' />
                
                <div class='form-group'>
                    <label for='letterInput'>Внесете буква:</label>
                    <input type='text' id='letterInput' name='letterInput' maxlength='1' required />
                </div>
                
                <button type='submit'>Провери</button>
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
    string rawPosition = form["randomPosition"];
    string rawInput = form["letterInput"].ToString().Trim();

    int position = int.Parse(rawPosition);
    char givenLetter = alphabet[position - 1];

    // Валидација: Внесот мора да биде точно една латинска буква
    if (string.IsNullOrEmpty(rawInput) || rawInput.Length != 1 || !char.IsLetter(rawInput[0]))
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync("<h2>Невалиден внес! Молиме внесете само една буква.</h2><a href='/'>← Назад</a>");
        return;
    }

    char userLetter = char.ToUpper(rawInput[0]);

    // Пресметка: Реден број поделен со 3 (Floor заокружување)
    int expectedPosition = (int)Math.Floor((double)position / 3);
    char expectedLetter = alphabet[expectedPosition - 1];

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
                <p>Дадена буква: <span class='highlight'>{givenLetter}</span> (Реден број: {position})</p>
                <p>Очекувана буква: <span id='mainresult' class='highlight'>{expectedLetter}</span></p>
                <p>Внесена буква: <span id='userresult' class='highlight'>{userLetter}</span></p>
            </div>
            <a href='/'>← Назад</a>
        </div>
    </body>
    </html>";

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(responseHtml);
});

app.Run();