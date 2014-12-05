using CodeMercury.Components;
using Example;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleClient
{
    class Program
    {
        static void Main()
        {
            AsyncContext.Run(() => MainAsync());
        }

        static Task MainAsync()
        {
            while (true)
            {
                string username = Console.ReadLine();
                Test(invoker);
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
