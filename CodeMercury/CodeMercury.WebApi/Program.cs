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
using System.Reactive.Disposables;

namespace CodeMercury.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StartMercuryNode(new Uri("http://localhost:9091/"), new Uri("http://localhost:9090")))
            {
                StartMercuryNode(new Uri("http://localhost:9090/"), new Uri("http://localhost:9091"), container =>
                {
                    var invoker = container.Resolve<HttpInvoker>();
                    var name = "Timothy";
                    //var task = invoker.InvokeAsync(() => BuildNameAsync(name));
                    var task = Test(invoker, name);
                    var result = task.WaitAndUnwrapException();
                    Console.WriteLine(result);
                    Console.ReadLine();
                });
            }
        }

        private static IDisposable StartMercuryNode(Uri requesterUri, Uri serverUri)
        {
            var container = new WindsorContainer();
            var installer = new WindsorInstaller(requesterUri, serverUri);
            container.Install(installer);
            var activator = new WindsorHttpControllerActivator(container);
            var startup = new Startup(activator);
            var app = WebApp.Start(requesterUri.ToString(), startup.Configuration);
            return new CompositeDisposable(app, container);
        }

        private static void StartMercuryNode(Uri requesterUri, Uri serverUri, Action<IWindsorContainer> run)
        {
            using (var container = new WindsorContainer())
            {
                var installer = new WindsorInstaller(requesterUri, serverUri);
                container.Install(installer);
                var activator = new WindsorHttpControllerActivator(container);
                var startup = new Startup(activator);
                using (WebApp.Start(requesterUri.ToString(), startup.Configuration))
                {
                    run(container);
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
