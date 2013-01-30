using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
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
                    MaxReceivedMessageSize = 1024*1024*1024,
                    TransferMode = TransferMode.Streamed,
                    MessageHandlers =
                    {
                        new ForwardProxyMessageHandler()
                    }
                };

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
