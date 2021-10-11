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

namespace Caravansary
{
    public static class Share
    {
        private static readonly WebClient client = new WebClient();

        private static readonly string port = "6090";
        private static readonly Uri address = new Uri("http://localhost:" + port);

        private static bool active = false;

        private static Queue<string> messageQueue = new Queue<string>();
        private static string processedMessage;
        private static bool clientInProgress = false;

        public static async void Start()
        {
            if (!active)
            {
                ServerStarter.ServerStarter.LaunchBorderless();
                active = true;
            }
            client.UploadStringCompleted += UploadCompleted;

            //GetS();

            //await t;
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
            messageQueue.Enqueue(message);

            using (var c = new WebClient())
            {
                c.Headers.Add("Content-Type", "text/json");

                processedMessage = messageQueue.Dequeue();

                var m = processedMessage.Split(":");
                if (m.Length < 2) return;

                c.UploadStringAsync(CreateValueEndpointAddress(m[0]), "POST", "\"" + m[1] + "\"");
            }

            //if (!clientInProgress)
            //    RunClient();
            //server.SetValue(message);
        }

        private static void RunClient()
        {
            if (messageQueue.Count <= 0) return;

            clientInProgress = true;

            processedMessage = messageQueue.Dequeue();

            var m = processedMessage.Split(":");
            if (m.Length < 2) return;

            client.UploadStringAsync(CreateValueEndpointAddress(m[0]), "POST", m[1]);
        }

        private static void UploadCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            clientInProgress = false;
            processedMessage = null;
            RunClient();
        }

        public static async void SendMessage(string message)
        {
            client.UploadStringAsync(address, message);
        }
    }
}