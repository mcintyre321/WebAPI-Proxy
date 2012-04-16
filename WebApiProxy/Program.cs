using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace WebApiProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HttpSelfHostServer server = null;
            try
            {
                var config = new HttpSelfHostConfiguration("http://localhost:3002/")
                {
                    MaxReceivedMessageSize = 1024 * 1024 * 1024,
                    TransferMode = TransferMode.Streamed,
                };

                config.Routes.MapHttpRoute("Proxy", "{*path}", new {controller = "Proxy", path = ""});

                server = new HttpSelfHostServer(config);

                server.OpenAsync().Wait();

                Console.WriteLine("Hit ENTER to exit");
                Console.ReadLine();
            }
            finally
            {
                if (server != null)
                {
                    server.CloseAsync().Wait();
                }
            }
        }
    }
}
