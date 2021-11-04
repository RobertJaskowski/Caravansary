<<<<<<< HEAD:Caravansary/Share.cs
﻿using System;
=======
﻿using Porter.Core;
using System;
>>>>>>> 1.4.5:Caravansary.Core/Share.cs
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD:Caravansary/Share.cs

using Newtonsoft.Json;
using System.Threading;
using System.Net.Sockets;
using System.Net.WebSockets;
=======
>>>>>>> 1.4.5:Caravansary.Core/Share.cs

namespace Caravansary
{
    public static class Share
    {
        //private static WsClient wsClient;
        private static readonly string port = "6091";

        //private static readonly Uri address = new Uri("http://localhost:" + port);
        private static readonly Uri address = new Uri("ws://localhost:" + port + "/ws");

        private static bool active = false;

        //private static Queue<string> messageQueue = new Queue<string>();
        private static string processedMessage;

        private static bool clientInProgress = false;
        private static bool connected = false;
        private static ClientWebSocket socket;

        public static async void Start()
        {
            if (!active)
            {
                //ServerStarter.ServerStarter.LaunchBorderless();
                ConnectSocket();
            }
            //client.UploadStringCompleted += UploadCompleted;

            //GetS();

            //await t;
        }

        private static void ConnectSocket()
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

        private static Uri CreateValueEndpointAddress(string endpoint)
        {
            return new Uri(address + "api/value/" + endpoint);
        }

        //private static async void GetS()
        //{
        //    var t = await client.DownloadStringTaskAsync(new Uri(address + "/activeTimer"));
        //}

        public static async void SendEvent(string jsonString)
        {
            SendToSocket(jsonString);
        }

        private static void OnConnected()
        {
            Console.WriteLine("connected");
            active = true;

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
            if (!connected) return;
            var buffer = Encoding.UTF8.GetBytes(msg);
            socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}