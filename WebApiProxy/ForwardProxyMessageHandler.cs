using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiProxy
{
    public class ForwardProxyMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Forwarded-For", request.GetClientIp());
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Trace) request.Content = null;
            request.RequestUri = new Uri(request.RequestUri.ToString().Replace(":3002", "")); //comes through with the port for the proxy, rewrite to port 80
            request.Headers.AcceptEncoding.Clear();
            var responseMessage = await new HttpClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            responseMessage.Headers.TransferEncodingChunked = null; //throws an error on calls to WebApi results
            if (request.Method == HttpMethod.Head) responseMessage.Content = null;
            return responseMessage;
        }

    }
}