using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApiProxy
{
    public class ProxyController : IHttpController
    {
        public Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            var request = controllerContext.Request;
            return request.Content.ReadAsStreamAsync().ContinueWith(s =>
            {
                var content = new StreamContent(s.Result);
                request.Content.CopyHeadersTo(content);
                return Proxy(controllerContext.RouteData.Values["path"] as string ?? "", request);
            }).Unwrap();
        }

        private Task<HttpResponseMessage> Proxy(string path, HttpRequestMessage request)
        {
            var httpClient = new HttpClient();
            bool useFiddler = true;
            var hostname = useFiddler ? "ipv4.fiddler:3000" : "localhost:3000"; //special address for fiddler as localhost doesn't get intercepted

            var apiRequest = new HttpRequestMessage(request.Method, new Uri("http://" + hostname + "/" + path));

            request.CopyHeadersTo(apiRequest);
            apiRequest.Headers.Host = hostname;
            return request.Content.ReadAsStreamAsync()
                .ContinueWith(t =>
                {
                    if (request.Content.Headers.ContentLength > 0)
                    {
                        apiRequest.Content = new StreamContent(t.Result);
                        request.Content.CopyHeadersTo(apiRequest.Content);
                    }
                    return httpClient.SendAsync(apiRequest);
                })
                .Unwrap()
                .ContinueWith(s => ReadApiResultContentAndReturnNewResponse(s))
                .Unwrap();
        }


        private Task<HttpResponseMessage> ReadApiResultContentAndReturnNewResponse(Task<HttpResponseMessage> apiResponseTask)
        {
            var apiResponse = apiResponseTask.Result;
            return apiResponse.Content.ReadAsStreamAsync().ContinueWith(t =>
            {
                var response = new HttpResponseMessage();
                apiResponse.CopyHeadersTo(response);
                response.Headers.TransferEncodingChunked = false;
                response.Content = new StreamContent(t.Result);
                apiResponse.Content.CopyHeadersTo(response.Content);
                return response;
            });
        }


    }
}