using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

    public class AddCommentHandler : DelegatingHandler
    {

        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken ct)
        {
            var innerResult = await base.SendAsync(request, ct);
            var content = await innerResult.Content.ReadAsStringAsync();
            var encodnig = innerResult.Content.Headers.ContentEncoding;
            innerResult.Content = new StringContent("hello" + content, new UnicodeEncoding() , innerResult.Content.Headers.ContentType.MediaType);
            return innerResult;


            //return base.SendAsync(request, ct).ContinueWith(
            //         task =>
            //         {
            //             var response = task.Result;
            //             var content = response.Content;
            //             if (content != null)
            //             {
            //                 return content.ReadAsStringAsync().ContinueWith(ca =>                            {
            //                     response.Content = new StringContent(ca.Result + "<hello />");
            //                     return response;
            //                 }).ContinueWith(ca2 => ca2.Result);

            //             }
            //             return response;
            //         }
            //     );
        }
    }
}