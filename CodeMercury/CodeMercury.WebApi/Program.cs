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
using CodeMercury.Services;
using CodeMercury.WebApi.Components;
using Newtonsoft.Json.Converters;

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
                var activator = new WindsorHttpControllerActivator(container);
                var startup = new Startup(activator);
                using (WebApp.Start(url, startup.Configuration))
                {
                    var invoker = container.Resolve<HttpInvoker>();
                    
                    var name = "Timothy";
                    //var task = invoker.InvokeAsync(() => BuildNameAsync(name));
                    var task = Test(invoker, name);
                    var result = task.WaitAndUnwrapException();
                    Console.WriteLine(result);

                    Console.ReadLine();
                }
            }
        }

        static async Task<string> BuildNameAsync(string name)
        {
            await Task.Yield();
            return string.Format("Hello, {0}!", name);
        }

        /// <summary>
        /// Test out some proxy behavior.
        /// </summary>
        /// <param name="invoker">The invoker supplied by code mercury.</param>
        /// <param name="username">The username to use in the greeting.</param>
        /// <returns>The content "Hello, {username}! You have unread messages."</returns>
        public static async Task<string> Test(IInvoker invoker, string username)
        {
            var gizmoCache = new LocalGizmoCache();
            var gizmoId = 7;
            await gizmoCache.PutGizmoAsync(new Gizmo(gizmoId, "You have unread messages.", true));
            await invoker.InvokeAsync(() => Runtime.MakeGizmoFriendly(gizmoId, username, gizmoCache));
            var gizmo = await gizmoCache.GetGizmoAsync(gizmoId);
            return gizmo.Content;
        }
    }
}
