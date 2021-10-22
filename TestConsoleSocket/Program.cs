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

        //private static void Main(string[] args)
        //{
        //    Socket s = new Socket(AddressFamily.InterNetwork,
        //        SocketType.Stream,
        //            ProtocolType.Tcp);

        //    Console.WriteLine("Establishing Connection to");

        //    DnsEndPoint dnsEndPoint = new DnsEndPoint("localhost", 6091);
        //    dnsEndPoint.Create(new SocketAddress(AddressFamily.InterNetwork));

        //    s.ConnectAsync(dnsEndPoint);
        //    Console.WriteLine("Connection established");
        //}
        private static ClientWebSocket socket;

        public static bool connected = false;

        public static void Main(string[] args)
        {
            string host = "localhost";
            int port = 6091;

            //if (args.Length == 0)
            //    // If no server name is passed as argument to this program,
            //    // use the current host name as the default.
            //    host = Dns.GetHostName();
            //else
            //    host = args[0];
            //string result = SocketSendReceive(host, port);
            //Console.WriteLine(result);

            socket = new ClientWebSocket();
            Task.Run(async () =>
            {
                await socket.ConnectAsync(address, CancellationToken.None);
                connected = true;
            });

            Task.Run(() =>
           {
               while (!connected)
                   Thread.Sleep(1000);

               Console.WriteLine("connected");

               var buffer = Encoding.UTF8.GetBytes("from client: test conenction establishemenet ");
               socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
           });

            string request = "GET / HTTP/1.1\r\nHost: " + host +
              "\r\nConnection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.UTF8.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];
            string result = "";

            //using (s = ConnectSocket(host, port).Result)
            //{
            //    if (s == null)
            //        Console.WriteLine("s null");
            //    else
            //    {
            //        // Send request to the server.
            //        s.Send(bytesSent, bytesSent.Length, 0);

            //        // Receive the server home page content.
            //        int bytes = 0;
            //        result = "Result on " + host + ":\r\n";

            //        // The following will block until the page is transmitted.
            //        do
            //        {
            //            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            //            result = result + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            //        }
            //        while (bytes > 0);
            //    }
            //}
            //Console.WriteLine(result);
            Console.ReadLine();

            //while (working)
            //{
            //    string line = Console.ReadLine();
            //    if (string.IsNullOrEmpty(line))
            //        break;

            //    if (line == "1")
            //    {
            //        Console.WriteLine("Client sending 1...");
            //        // client.SendTextAsync("asd1");
            //    }

            //    // Reconnect the client
            //    if (line == "!")
            //    {
            //        Console.Write("Client reconnecting...");
            //        //if (client.IsConnected)
            //        //    client.ReconnectAsync();
            //        //else
            //        //    client.ConnectAsync();
            //        Console.WriteLine("Done!");
            //        continue;
            //    }

            // Send the entered text to the chat server
            // client.SendTextAsync(line);
            //}
        }

        private async static Task<Socket> ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            Socket tm = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await tm.ConnectAsync(server, port);

            return tm;

            //// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            //// an exception that occurs when the host IP Address is not compatible with the address family
            //// (typical in the IPv6 case).
            //foreach (IPAddress address in hostEntry.AddressList)
            //{
            //    IPEndPoint ipe = new IPEndPoint(address, port);
            //    var af = ipe.AddressFamily;
            //    Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //    tempSocket.Connect(server, port);

            //    if (tempSocket.Connected)
            //    {
            //        s = tempSocket;
            //        break;
            //    }
            //    else
            //    {
            //        continue;
            //    }
            //}
            //return s;
        }

        // This method requests the home page content for the specified server.
        private static string SocketSendReceive(string server, int port)
        {
            string request = "GET / HTTP/1.1\r\nHost: " + server +
                "\r\nConnection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];
            string page = "";

            // Create a socket connection with the specified server and port.
            using (Socket s = ConnectSocket(server, port).Result)
            {
                if (s == null)
                    return ("Connection failed");

                // Send request to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the server home page content.
                int bytes = 0;
                page = "Default HTML page on " + server + ":\r\n";

                // The following will block until the page is transmitted.
                do
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
            }

            return page;
        }

        //    private static void Main(string[] args)
        //    {
        //        // WebSocket server address
        //        string address = "127.0.0.1";
        //        if (args.Length > 0)
        //            address = args[0];

        //        // WebSocket server port
        //        int port = 6091;
        //        if (args.Length > 1)
        //            port = int.Parse(args[1]);

        //        Console.WriteLine($"WebSocket server address: {address}");
        //        Console.WriteLine($"WebSocket server port: {port}");

        //        Console.WriteLine();

        //        //var context = new SslContext(SslProtocols.Tls12, new X509Certificate2("cer.pfx", "qwerty"), (sender, certificate, chain, sslPolicyErrors) => false);
        //        var context = new SslContext(SslProtocols.None);

        //        var client = new ChatClient(address, port);

        //        Console.Write("Client connecting...");
        //        client.ConnectAsync();
        //        Console.WriteLine("Done!");

        //        Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

        //        // Perform text input
        //        while (working)
        //        {
        //            string line = Console.ReadLine();
        //            if (string.IsNullOrEmpty(line))
        //                break;

        //            if (line == "1")
        //            {
        //                Console.WriteLine("Client sending 1...");
        //                client.SendTextAsync("asd1");
        //            }

        //            // Reconnect the client
        //            if (line == "!")
        //            {
        //                Console.Write("Client reconnecting...");
        //                if (client.IsConnected)
        //                    client.ReconnectAsync();
        //                else
        //                    client.ConnectAsync();
        //                Console.WriteLine("Done!");
        //                continue;
        //            }

        //            // Send the entered text to the chat server
        //            // client.SendTextAsync(line);
        //        }

        //        // Disconnect the client
        //        Console.Write("Client disconnecting...");
        //        //client.DisconnectAndStop();
        //        client.Disconnect();

        //        Console.WriteLine("Done!");
        //    }
        //}

        //internal class ChatClient : WsClient
        //{
        //    public ChatClient(string address, int port) : base(address, port)
        //    {
        //    }

        //    public void DisconnectAndStop()
        //    {
        //        _stop = true;
        //        CloseAsync(1000);
        //        while (IsConnected)
        //            Thread.Yield();
        //    }

        //    public override void OnWsConnecting(HttpRequest request)
        //    {
        //        request.SetBegin("GET", "/","ws");
        //        request.SetHeader("Host", "localhost");
        //        request.SetHeader("Origin", "http://localhost");
        //        request.SetHeader("Upgrade", "websocket");
        //        request.SetHeader("Connection", "Upgrade");
        //        request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
        //        request.SetHeader("Sec-WebSocket-Protocol", "chat, superchat");
        //        request.SetHeader("Sec-WebSocket-Version", "13");
        //    }

        //    public override void OnWsConnected(HttpResponse response)
        //    {
        //        Console.WriteLine($"Chat WebSocket client connected a new session with Id {Id}");
        //    }

        //    public override void OnWsDisconnected()
        //    {
        //        Console.WriteLine($"Chat WebSocket client disconnected a session with Id {Id}");
        //    }

        //    public override void OnWsReceived(byte[] buffer, long offset, long size)
        //    {
        //        Console.WriteLine($"Incoming: {Encoding.UTF8.GetString(buffer, (int)offset, (int)size)}");
        //    }

        //    protected override void OnDisconnected()
        //    {
        //        base.OnDisconnected();

        //        Console.WriteLine($"Chat WebSocket client disconnected a session with Id {Id}");

        //        // Wait for a while...
        //        Thread.Sleep(1000);

        //        // Try to connect again
        //        if (!_stop)
        //            ConnectAsync();
        //    }

        //    protected override void OnError(SocketError error)
        //    {
        //        Console.WriteLine($"Chat WebSocket client caught an error with code {error}");
        //    }

        //    private bool _stop;
        //}
    }
}