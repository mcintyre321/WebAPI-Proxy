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
            //var host = "localhost:3000";
            //request.Headers.Host = host;
            request.Headers.Add("X-Forwarded-For", request.GetClientIp());
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Trace) request.Content = null;
            
//            request.RequestUri = request.RequestUri;
            request.RequestUri = new Uri(request.RequestUri.ToString().Replace(":3002", ""));
            //request.RequestUri = new Uri("http://" + host + request.RequestUri.PathAndQuery);
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