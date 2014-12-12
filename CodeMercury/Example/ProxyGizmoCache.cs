using CodeMercury.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class ProxyGizmoCache : IGizmoCache, IProxy<IGizmoCache>
    {
        private readonly IInvoker invoker;

        public ProxyGizmoCache(IInvoker invoker)
        {
            this.invoker = invoker;
        }

        public async Task<Gizmo> GetGizmoAsync(int id)
        {
            return await invoker.InvokeAsync(() => GetGizmoAsync(id));
        }

        public async Task PutGizmoAsync(Gizmo gizmo)
        {
            await invoker.InvokeAsync(() => PutGizmoAsync(gizmo));
        }
    }
}
