using Castle.Windsor;
using Microsoft.Owin;
using Microsoft.Owin.BuilderProperties;
using Owin;
using System;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace CodeMercury.WebApi
{
    public class Startup
    {
        private readonly IHttpControllerActivator activator;

        public Startup(IHttpControllerActivator activator)
        {
            if (activator == null)
            {
                throw new ArgumentNullException("activator");
            }

            this.activator = activator;
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Services.Replace(typeof(IHttpControllerActivator), activator);
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }
}