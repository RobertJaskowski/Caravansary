using NetCoreServer;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsoleSocket
{
    internal class Program
    {
        private static readonly string port = "6091";
        private static readonly Uri address = new Uri("ws://localhost:" + port + "/ws");

        private static bool working = true;

        private static ClientWebSocket socket;

        public static bool connected = false;

        public static void Main(string[] args)
        {
            string host = "localhost";
            int port = 6091;

            socket = new ClientWebSocket();
            Connect(() => { OnConnected(); });

            string line = "";
            while (line != "q")
            {
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                if (line == "1")
                {
                    Console.WriteLine("Client sending 1...");

                    SendToSocket("test data on 1");
                }

                if (line == "!")
                {
                    Console.Write("Client reconnecting...");

                    Console.WriteLine("Done!");
                    continue;
                }
            }
        }

        private static void OnConnected()
        {
            Console.WriteLine("connected");
            StartReceiveLoopAsync();
        }

        private static void StartReceiveLoopAsync()
        {
            Task.Run(async () =>
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = null;
                string stringResult = null;

                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    stringResult = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.WriteLine("Message received from Client: " + stringResult);
                } while (!result.CloseStatus.HasValue);

                return Task.CompletedTask;
            });
        }

        private static void Connect(Action onConnected)
        {
            Task.Run(async () =>
            {
                await socket.ConnectAsync(address, CancellationToken.None);
                connected = true;
                onConnected?.Invoke();
            });
        }

        private static void SendToSocket(string msg)
        {
            var buffer = Encoding.UTF8.GetBytes(msg);
            socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}