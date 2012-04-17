using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApiProxy
{
    public class ProxyController : IHttpController
    {
        public Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,
                                                      CancellationToken cancellationToken)
        {
            var request = controllerContext.Request;
            var host = "localhost:3000";
            request.Headers.Host = host;
            request.Headers.Add("X-Forwarded-For", request.GetClientIp());
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Trace) request.Content = null;
            request.RequestUri = new Uri("http://" + host + request.RequestUri.PathAndQuery);

            return new HttpClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ContinueWith(r =>
                {
                    r.Result.Headers.TransferEncodingChunked = null; //throws an error on calls to WebApi results

                    if (request.Method == HttpMethod.Head) r.Result.Content = null;
                    return r.Result;
                });
        }
    }
}