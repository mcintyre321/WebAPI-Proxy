using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Services;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using WebApiProxy;
using WebApiProxy.Authorization;

namespace AspHostedWebApiProxy
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterApis(GlobalConfiguration.Configuration);
        }

        public static void RegisterApis(HttpConfiguration config)
        {
            GlobalConfiguration.Configuration.MessageHandlers.Add(new ValidateTokenHandler()); //uncomment to require header Authorization: Harry
            config.Routes.MapHttpRoute("default", "{*path}", new { controller = "Proxy", path = "" });
        }
    }
}