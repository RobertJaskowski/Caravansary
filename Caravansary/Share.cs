using SocketIOClient.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace Caravansary
{
    public static class Share
    {
        private static readonly WebClient client = new WebClient();

        //private static WsClient wsClient;
        private static readonly string port = "6091";

        //private static readonly Uri address = new Uri("http://localhost:" + port);
        private static readonly Uri address = new Uri("ws://localhost:" + port);

        private static bool active = false;

        //private static Queue<string> messageQueue = new Queue<string>();
        private static string processedMessage;

        private static bool clientInProgress = false;
        private static bool connected = false;

        public static async void Start()
        {
            if (!active)
            {
                //ServerStarter.ServerStarter.LaunchBorderless();
                ConnectSocket();
                active = true;
            }
            //client.UploadStringCompleted += UploadCompleted;

            //GetS();

            //await t;
        }

        private static SocketIO ws = null;

        private static async void ConnectSocket()
        {
            ws = new SocketIO(address.ToString());
            ws.OnConnected += OnConnected;

            connected = true;

            await ws.ConnectAsync();

            Console.WriteLine("on after connect await");
            //wsClient = new WsClient();
            //await wsClient.ConnectAsync(address.ToString());
        }

        private static void OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString() + " " + e.ToString());
            ws.EmitAsync("testEvent");
        }

        private static void SendMsgAssync(string msg)
        {
            ws.EmitAsync("testEvent", msg);
        }

        private static Uri CreateValueEndpointAddress(string endpoint)
        {
            return new Uri(address + "api/value/" + endpoint);
        }

        private static async void GetS()
        {
            var t = await client.DownloadStringTaskAsync(new Uri(address + "/activeTimer"));
        }

        public static async void SetValue(string message)
        {
            //messageQueue.Enqueue(message);

            //if (!wsClient.connected) return;
            if (!connected) return;

            using (var c = new WebClient())
            {
                c.Headers.Add("Content-Type", "text/json");

                //processedMessage = messageQueue.Dequeue();
                processedMessage = message;

                var m = processedMessage.Split(":");
                if (m.Length < 2) return;

                SendMsgAssync(message);

                // wsClient.SendMessageAsync(message);

                //c.UploadStringAsync(CreateValueEndpointAddress(m[0]), "POST", "\"" + m[1] + "\"");
            }

            //if (!clientInProgress)
            //    RunClient();
            //server.SetValue(message);
        }

        //private static void RunClient()
        //{
        //    if (messageQueue.Count <= 0) return;

        //    clientInProgress = true;

        //    processedMessage = messageQueue.Dequeue();

        //    var m = processedMessage.Split(":");
        //    if (m.Length < 2) return;

        //    client.UploadStringAsync(CreateValueEndpointAddress(m[0]), "POST", m[1]);
        //}

        //private static void UploadCompleted(object sender, UploadStringCompletedEventArgs e)
        //{
        //    clientInProgress = false;
        //    processedMessage = null;
        //    RunClient();
        //}

        public static async void SendMessage(string message)
        {
            client.UploadStringAsync(address, message);
        }
    }

    //public class WsClient : IDisposable
    //{
    //    public bool connected = false;
    //    public int ReceiveBufferSize { get; set; } = 8192;

    //    public async Task ConnectAsync(string url)
    //    {
    //        if (WS != null)
    //        {
    //            if (WS.State == WebSocketState.Open) return;
    //            else WS.Dispose();
    //        }
    //        WS = new ClientWebSocket();
    //        if (CTS != null) CTS.Dispose();
    //        CTS = new CancellationTokenSource();
    //        await WS.ConnectAsync(new Uri(url), CTS.Token);
    //        Console.WriteLine("Connected socket ");
    //        connected = true;
    //        await Task.Factory.StartNew(ReceiveLoop, CTS.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    //    }

    //    public async Task DisconnectAsync()
    //    {
    //        if (WS is null) return;
    //        // TODO: requests cleanup code, sub-protocol dependent.
    //        if (WS.State == WebSocketState.Open)
    //        {
    //            CTS.CancelAfter(TimeSpan.FromSeconds(2));
    //            await WS.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
    //            await WS.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
    //        }
    //        WS.Dispose();
    //        WS = null;
    //        CTS.Dispose();
    //        CTS = null;
    //    }

    //    private async Task ReceiveLoop()
    //    {
    //        var loopToken = CTS.Token;
    //        MemoryStream outputStream = null;
    //        WebSocketReceiveResult receiveResult = null;
    //        var buffer = new byte[ReceiveBufferSize];

    //        try
    //        {
    //            while (!loopToken.IsCancellationRequested)
    //            {
    //                outputStream = new MemoryStream(ReceiveBufferSize);
    //                do
    //                {
    //                    receiveResult = await WS.ReceiveAsync(buffer, CTS.Token);
    //                    if (receiveResult.MessageType != WebSocketMessageType.Close)
    //                        outputStream.Write(buffer, 0, receiveResult.Count);
    //                }
    //                while (!receiveResult.EndOfMessage);
    //                if (receiveResult.MessageType == WebSocketMessageType.Close) break;
    //                outputStream.Position = 0;
    //                ResponseReceived(outputStream);
    //            }
    //        }
    //        catch (TaskCanceledException) { }
    //        finally
    //        {
    //            outputStream?.Dispose();
    //        }
    //    }

    //    public async Task SendMessageAsync(string message)
    //    {
    //        // TODO: handle serializing requests and deserializing responses, handle matching responses to the requests.
    //        var buffer = new byte[1024 * 4];

    //        string result = message;

    //        MemoryStream ms = new MemoryStream();
    //        ms.Write(buffer);

    //        await WS.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, false, CancellationToken.None);
    //    }

    //    private void ResponseReceived(Stream inputStream)
    //    {
    //        StreamReader sr = new StreamReader(inputStream);
    //        var s = sr.ReadToEnd();
    //        Console.WriteLine(s);

    //        inputStream.Dispose();
    //        // TODO: handle deserializing responses and matching them to the requests.
    //        // IMPORTANT: DON'T FORGET TO DISPOSE THE inputStream!
    //    }

    //    public void Dispose() => DisconnectAsync().Wait();

    //    private ClientWebSocket WS;
    //    private CancellationTokenSource CTS;
    //}
}