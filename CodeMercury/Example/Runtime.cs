using CodeMercury.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public static class Runtime
    {
        public static async Task MakeGizmoFriendly(int gizmoId, string username, IGizmoCache gizmoCache)
        {
            var gizmo = await gizmoCache.GetGizmoAsync(gizmoId);
            var content = string.Format("Hello, {0}! {1}", username, gizmo.Content);
            await gizmoCache.PutGizmoAsync(new Gizmo(gizmo.Id, content, gizmo.IsWidget));
        }
    }
}
