using CodeMercury.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class ProxyGizmoCache : IGizmoCache
    {
        private readonly IGizmoCache remote;
        private readonly IInvoker invoker;

        public ProxyGizmoCache(IGizmoCache remote, IInvoker invoker)
        {
            this.remote = remote;
            this.invoker = invoker;
        }

        public async Task<Gizmo> GetGizmoAsync(int id)
        {
            return await invoker.InvokeAsync(() => remote.GetGizmoAsync(id));
        }

        public async Task PutGizmoAsync(Gizmo gizmo)
        {
            await invoker.InvokeAsync(() => remote.PutGizmoAsync(gizmo));
        }
    }
}
