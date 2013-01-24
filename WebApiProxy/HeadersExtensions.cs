using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiProxy
{
    static class HeadersExtensions
    {
        internal static void CopyTo(this HttpRequestHeaders from, HttpRequestHeaders to)
        {
            foreach (var header in from)
            {
                to.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        internal static void CopyHeadersTo(this HttpRequestMessage from, HttpRequestMessage to)
        {
            from.Headers.CopyTo(to.Headers);
        }


        internal static void CopyTo(this HttpContentHeaders from, HttpContentHeaders to)
        {
            foreach (var header in from)
            {
                to.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        internal static void CopyHeadersTo(this HttpContent from, HttpContent to)
        {
            from.Headers.CopyTo(to.Headers);
        }


        internal static void CopyTo(this HttpResponseHeaders from, HttpResponseHeaders to)
        {
            foreach (var header in from)
            {
                to.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        internal static void CopyHeadersTo(this HttpResponseMessage from, HttpResponseMessage to)
        {
            from.Headers.CopyTo(to.Headers);
        }
    }
}