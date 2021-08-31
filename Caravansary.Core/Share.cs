using Porter.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
{
    public static class Share
    {
        private static readonly WebClient client = new WebClient();

        public static Server server;
        private static readonly string port = "1337";
        private static readonly Uri address = new Uri("http://localhost:" + port);

        public static async void Start()
        {
            if (server != null) return;

            server = new Server();
            var t = server.Init();

            //GetS();

            //await t;

            //todo globalizing assemblies to not reproduce servers across multiple applications
        }

        private static async void GetS()
        {
            //var t = await client.GetAsync("http://localhost:1337/users/44");
            var t = await client.DownloadStringTaskAsync(new Uri("http://localhost:1337/value/activeTimer"));
        }

        public static void SetValue(string message)
        {
            server.SetValue(message);
        }

        public static async void SendMessage(string message)
        {
            client.UploadStringAsync(address, message);
        }
    }
}