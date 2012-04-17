using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;

namespace WebApiProxy.Authorization
{
    public class IPHostValidationHandler : DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {

            //var context = request.Properties["MS_HttpContext"] as System.Web.HttpContextBase;
            //string userIP = context.Request.UserHostAddress;
            //try
            //{
            //    AuthorizedIPRepository.GetAuthorizedIPs().First(x => x == userIP);
            //}
            //catch (Exception)
            //{
            //    actionContext.Response =
            //       new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
            //       {
            //           Content = new StringContent("Unauthorized IP Address")
            //       };
            //    return;
            //}
            return base.SendAsync(request, cancellationToken);
        }
    }
}
