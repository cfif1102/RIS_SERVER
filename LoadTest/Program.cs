using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static int _processedClients = 0;

    public static async Task Main(string[] args)
    {
        int clientCount = 10000;
        string serverUri = "ws://127.0.0.1:8888";

        Console.WriteLine("Запуск теста...");

        Stopwatch stopwatch = Stopwatch.StartNew();

        Task[] tasks = new Task[clientCount];
        for (int i = 0; i < clientCount; i++)
        {
            tasks[i] = Task.Run(() => RunClient(serverUri, i));
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();

        Console.WriteLine("Тест завершен.");
        Console.WriteLine($"Обработано клиентов: {_processedClients}");
        Console.WriteLine($"Суммарное время обработки: {stopwatch.ElapsedMilliseconds} ms");
    }

    static async Task RunClient(string uri, int clientId)
    {
        using (var client = new ClientWebSocket())
        {
            try
            {
                await client.ConnectAsync(new Uri(uri), CancellationToken.None);

                string message = JsonSerializer.Serialize(new
                {
                    Action = "user/find-many"
                });

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                var buffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Interlocked.Increment(ref _processedClients);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
