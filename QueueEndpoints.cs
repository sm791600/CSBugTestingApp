using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public static class QueueEndpoints
{
    private static int globalCounter = int.MaxValue - 1000;
    private static bool isCounterRunning = false;
    private static string globalStatusMessage = "Системот процесира барања...";

    public static void MapQueueEndpoints(WebApplication app)
    {
        app.MapGet("/queue-status", async (HttpContext context) =>
        {
            StartQueueCounter();

            string html = await File.ReadAllTextAsync("Views/QueueStatus.html");
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        });

        app.MapGet("/api/queue-data", () =>
        {
            return Results.Json(new
            {
                counter = globalCounter,
                status = globalStatusMessage
            });
        });
    }

    private static void StartQueueCounter()
    {
        if (isCounterRunning) return;
        isCounterRunning = true;

        Task.Run(() =>
        {
            int previousValue = 0;
            unchecked
            {
                while (true)
                {
                    previousValue = globalCounter;
                    globalCounter++;

                    if (globalCounter < previousValue)
                    {
                        globalStatusMessage = "Системска грешка при обработка на редот!";
                    }

                    Thread.Sleep(10);
                }
            }
        });
    }
}