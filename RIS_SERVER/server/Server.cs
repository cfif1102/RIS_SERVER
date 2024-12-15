using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RIS_SERVER.server
{
    public class Server
    {
        private readonly SemaphoreSlim Semaphore = new(1, 1);
        public static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
        private readonly Handler _handler;

        public Server(Handler handler)
        {
            _handler = handler;
        }

        public string SerializeResponse(object response)
        {
            var json = JsonSerializer.Serialize(response, options);

            return json;
        }

        public void Start()
        {
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://127.0.0.1:8888/");
            listener.Start();

            Console.WriteLine("Server is started...");

            while (true)
            {
                try
                {

                    HttpListenerContext context = listener.GetContext();

                    if (context.Request.IsWebSocketRequest)
                    {
                        WebSocketContext wsContext = context.AcceptWebSocketAsync(null).Result;


                        ThreadPool.QueueUserWorkItem(_ => HandleWebSocketConnection(wsContext.WebSocket));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                } catch (Exception ex)
                {
                }
            }
        }

        private void HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            var receivedData = new StringBuilder();
            WebSocketReceiveResult result;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    do
                    {
                        try
                        {
                            result = webSocket
                                .ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                                .GetAwaiter()
                                .GetResult();
                        }
                        catch (WebSocketException ex)
                        {
                            // Клиент завершил соединение некорректно.
                            Console.WriteLine($"WebSocketException: {ex.Message}");
                            return;
                        }

                        if (result.Count == 0) break;

                        receivedData.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    } while (!result.EndOfMessage);

                    string jsonMessage = receivedData.ToString();

                    if (string.IsNullOrWhiteSpace(jsonMessage))
                    {
                        continue;
                    }

                    var clientRequest = JsonSerializer.Deserialize<ClientRequest>(jsonMessage, options);
                    var action = clientRequest.Action;

                    Console.WriteLine($"Received {action} from client...");

                    try
                    {
                        Semaphore.Wait();

                        var response = _handler.Run(clientRequest);

                        Semaphore.Release();

                        Send(response, webSocket);

                        receivedData.Clear();
                    }
                    catch (WebSocketException)
                    {
                        if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                        {
                            webSocket
                                .CloseAsync(WebSocketCloseStatus.NormalClosure, "Соединение закрыто сервером", CancellationToken.None)
                                .Wait();
                        }
                        return;
                    }
                    catch (WsException ex)
                    {
                        Semaphore.Release();

                        var message = new
                        {
                            Action = $"{action}-error",
                            Data = new
                            {
                                Status = ex.ErrorCode,
                                Message = ex.CustomMessage,
                            }
                        };

                        Send(message, webSocket);

                        receivedData.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            finally
            {
                if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
                {
                    try
                    {
                        webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();
                    }
                    catch
                    {
                    }
                }
                webSocket.Dispose();
                Console.WriteLine("WebSocket connection closed.");
            }
        }


        private void Send(object obj, WebSocket webSocket)
        {
            var json = JsonSerializer.Serialize(obj, options);

            webSocket
                .SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None)
                .Wait();
        }
    }
}
