using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class LocalGizmoCache : IGizmoCache
    {
        private object sync = new object();
        private IDictionary<int, Gizmo> dictionary = new Dictionary<int, Gizmo>();

        public async Task<Gizmo> GetGizmoAsync(int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            lock (sync)
            {
                return dictionary[id];
            }
        }

        public async Task PutGizmoAsync(Gizmo gizmo)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            lock (sync)
            {
                dictionary[gizmo.Id] = gizmo;
            }
        }
    }
}
