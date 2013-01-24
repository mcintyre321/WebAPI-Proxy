using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.SelfHost;
using WebApiProxy.Authorization;

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
                    MessageHandlers =
                        {
                            //new ValidateTokenHandler(),
                            //new IPHostValidationHandler()
                        }
                };
                config.MessageHandlers.Clear();
                config.MessageHandlers.Add(new AddCommentHandler());
                config.MessageHandlers.Add(new ForwardProxyMessageHandler());

                config.Routes.MapHttpRoute("Proxy", "{*path}", new {controller = "Proxy", path = ""});

                server = new HttpSelfHostServer(config);

                server.OpenAsync().Wait();

                var timer = new Timer(Report, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
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

        private static void Report(object state)
        {
            var pos = new {Console.CursorLeft, Console.CursorTop};
            Console.SetCursorPosition(0,0);
            Console.Write(string.Format("{0:n0}", Process.GetCurrentProcess().PagedMemorySize64 / 1024) + "kb            ");
            Console.SetCursorPosition(pos.CursorLeft, pos.CursorTop);
        }
    }
    
 
}
