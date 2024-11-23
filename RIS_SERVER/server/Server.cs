using RIS_SERVER.src.common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RIS_SERVER.server
{
    public class Server
    {
        private readonly ConcurrentDictionary<string, WebSocket> Clients = new();
        private readonly SemaphoreSlim Semaphore = new(1, 1);
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();

                if (context.Request.IsWebSocketRequest)
                {
                    WebSocketContext wsContext = context.AcceptWebSocketAsync(null).Result;

                    string clientId = Guid.NewGuid().ToString(); 

                    Clients[clientId] = wsContext.WebSocket;

                    Console.WriteLine($"Клиент подключен: {clientId}");

                    ThreadPool.QueueUserWorkItem(_ => HandleWebSocketConnection(clientId, wsContext.WebSocket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private void HandleWebSocketConnection(string clientId, WebSocket webSocket)
        {
            byte[] buffer = new byte[1024 * 4];
            MemoryStream binaryStream = new MemoryStream();
            FileMetadataDto fileMetadata = null;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        fileMetadata = JsonSerializer.Deserialize<FileMetadataDto>(jsonMessage, options);
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        binaryStream.Write(buffer, 0, result.Count);
                    }

                    if (result.EndOfMessage)
                    {
                        SaveFile(clientId, binaryStream.ToArray(), fileMetadata);
                        binaryStream.SetLength(0);
                    }

                    if (result.CloseStatus.HasValue)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка у клиента {clientId}: {ex.Message}");
            }
            finally
            {
                binaryStream.Dispose();

                if (webSocket.State == WebSocketState.Open)
                {
                    webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", CancellationToken.None).Wait();
                }
            }
        }

        private void SaveFile(string clientId, byte[] data, FileMetadataDto metadata)
        {
            string mediaDirectory = Path.Combine(AppContext.BaseDirectory, "media");

            if (!Directory.Exists(mediaDirectory))
            {
                Directory.CreateDirectory(mediaDirectory);
            }

            string uniqueFileName = $"{Guid.NewGuid()}_{metadata.Name}";
            string filePath = Path.Combine(mediaDirectory, uniqueFileName);

            File.WriteAllBytes(filePath, data);
        }
    }
}
