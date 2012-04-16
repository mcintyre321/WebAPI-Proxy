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
            GlobalConfiguration.Configuration.ServiceResolver.SetResolver(new DelegateBasedDependencyResolver());
            config.Routes.MapHttpRoute("default", "{*path}", new {controller = "Proxy", path = ""});

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
    public class DelegateBasedDependencyResolver : IDependencyResolver
    {
        public Dictionary<Type, Func<object>> Resolvers = new Dictionary<Type, Func<object>>();

        public object GetService(Type serviceType)
        {
            return Resolvers.ContainsKey(serviceType) ? Resolvers[serviceType]() : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Resolvers.Keys
                .Where(serviceType.IsAssignableFrom)
                .Select(t => Resolvers[t]());
        }
    }
}