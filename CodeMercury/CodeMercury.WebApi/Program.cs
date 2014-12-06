using Castle.Windsor;
using CodeMercury.Components;
using CodeMercury.Expressions;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using Nito.AsyncEx.Synchronous;
using System.Threading.Tasks;
using Example;

namespace CodeMercury.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new WindsorContainer())
            {
                container.Install(new WindsorInstaller());

                var port = 9090;// int.Parse(args[0]);
                var url = string.Format("http://localhost:{0}/", port);
                var activator = new WindsorCompositionRoot(container);
                var startup = new Startup(activator);
                using (WebApp.Start(url, startup.Configuration))
                {
                    var invoker = container.Resolve<HttpInvoker>();
                    
                    var name = "Timothy";
                    var task = invoker.InvokeAsync(() => string.Format("Hello, {0}!", name));
                    var result = task.WaitAndUnwrapException();
                    Console.WriteLine(result);

                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Test out some proxy behavior.
        /// </summary>
        /// <param name="invoker">The invoker supplied by code mercury.</param>
        /// <param name="username">The username to use in the greeting.</param>
        /// <returns>The content "Hello, Timothy! You have unread messages."</returns>
        public static async Task<string> Test(IInvoker invoker, string username)
        {
            var gizmoCache = new LocalGizmoCache();
            var gizmoId = 7;
            await gizmoCache.PutGizmoAsync(new Gizmo(gizmoId, "You have unread messages.", true));
            await invoker.InvokeAsync(() => Runtime.MakeGizmoFriendly(7, "Timothy", gizmoCache));
            var gizmo = await gizmoCache.GetGizmoAsync(gizmoId);
            return gizmo.Content;
        }
    }
}
