using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiProxy.Authorization
{
    
    public class ValidateTokenHandler : DelegatingHandler
    {
        static HashSet<string> users = new HashSet<string>()
        {
            "Harry" //RsaClass.Encrypt("Harry")
        };
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() =>
                    new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Missing Authorization-Token")
                    }
                );
            }


            var authorized = request.Headers.GetValues("Authorization")
                //.Select(RsaClass.Decrypt) uncomment to require encryption
                .Any(token => users.Contains(token));
            if (!authorized)
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() =>
                    new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("Unauthorized User")
                    }
                );
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}