using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiProxy
{
    public class ForwardProxyMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Forwarded-For", request.GetClientIp());
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Trace) request.Content = null;

            request.RequestUri = new Uri(request.RequestUri.ToString().Replace(":3002", "")); //comes through with the port for the proxy, rewrite to port 80
            
            
            request.Headers.AcceptEncoding.Clear();
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