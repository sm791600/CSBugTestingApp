using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(WebApplication app)
    {
        app.MapGet("/reservation", async (HttpContext context) =>
        {
            string html = await File.ReadAllTextAsync("Views/Reservation.html");
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        });

        app.MapPost("/enable-reservation", async () =>
        {
            double delaySeconds = Random.Shared.NextDouble() * (7.0 - 4.0) + 4.0;
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

            return Results.Json(new { status = "ready", delay = Math.Round(delaySeconds, 2) });
        });

        app.MapPost("/make-reservation", async () =>
        {
            double delaySeconds = Random.Shared.NextDouble() * (7.0 - 6.0) + 6.0;
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

            return Results.Json(new { status = "success", message = "Успешна резервација", delay = Math.Round(delaySeconds, 2) });
        });
    }
}