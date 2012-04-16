using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Web.Http.SelfHost;

namespace ConsoleWebApiProxy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HttpSelfHostServer server = null;

            try
            {
                var config = new HttpSelfHostConfiguration("http://localhost:3001/")
                {
                    MaxReceivedMessageSize = 1024*1024,
                    TransferMode = TransferMode.Streamed,
                };

                config.Routes.MapHttpRoute(name: "default", routeTemplate: "{*path}",
                                           defaults: new {controller = "ApiProxy", path = ""});
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

    public class ApiProxyController : ApiController
    {
        public Task<HttpResponseMessage> Get(string path)
        {
            return Proxy(path, null);
        }

       

        public Task<HttpResponseMessage> Post(string path)
        {
            return Request.Content.ReadAsStringAsync().ContinueWith(s =>
            {
                var content = new StringContent(s.Result);
                    CopyHeaders(Request.Content.Headers, content.Headers);
                return Proxy(path, content);
            }).Unwrap();
        }

        public Task<HttpResponseMessage> Put(string path)
        {
            return Proxy(path, null);
        }

        public Task<HttpResponseMessage> Delete(string path)
        {
            return Proxy(path, null);
        }


        private Task<HttpResponseMessage> Proxy(string path, HttpContent content)
        {
            var httpClient = new HttpClient();
            bool useFiddler = true;
            var hostname = useFiddler ? "ipv4.fiddler:3000" : "localhost:3000";


            var apiRequest = new HttpRequestMessage(Request.Method, new Uri("http://" + hostname + "/" + path));

            CopyHeaders(Request.Headers, apiRequest.Headers);
            apiRequest.Headers.Host = hostname;
            return Request.Content.ReadAsStringAsync()
                .ContinueWith(t =>
                {
                    if (Request.Content.Headers.ContentLength > 0)
                    {
                        apiRequest.Content = new StringContent(t.Result);
                        CopyHeaders(Request.Content.Headers, apiRequest.Content.Headers);
                    }
                    return httpClient.SendAsync(apiRequest);
                })
                .Unwrap()
                .ContinueWith(s => ReadApiResultContentAndReturnNewResponse(s))
                .Unwrap();

        }

        private static void CopyHeaders(HttpRequestHeaders from, HttpRequestHeaders to)
        {
            foreach (var header in from)
            {
                to.AddWithoutValidation(header.Key, header.Value);
            }
        }

        private static void CopyHeaders(HttpContentHeaders from, HttpContentHeaders to)
        {
            foreach (var header in from)
            {
                to.AddWithoutValidation(header.Key, header.Value);
            }
        }

        private static void CopyHeaders(HttpResponseHeaders from, HttpResponseHeaders to)
        {
            foreach (var header in from)
            {
                to.AddWithoutValidation(header.Key, header.Value);
            }
        }

        private Task<HttpResponseMessage> ReadApiResultContentAndReturnNewResponse(Task<HttpResponseMessage> apiResponseTask)
        {
            var apiResponse = apiResponseTask.Result;
            return apiResponse.Content.ReadAsStreamAsync().ContinueWith(t =>
            {
                var response = new HttpResponseMessage();

                CopyHeaders(apiResponse.Headers, response.Headers);
                response.Headers.TransferEncodingChunked = false;
                response.Content = new StreamContent(t.Result);
                CopyHeaders(apiResponse.Content.Headers, response.Content.Headers);
                return response;
            });

        }

    }
}