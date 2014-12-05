using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class CompositeInvoker : IInvoker
    {
        private readonly IList<IInvoker> invokers;
        private readonly Random random = new Random();

        public CompositeInvoker(IEnumerable<IInvoker> invokers)
        {
            this.invokers = invokers.ToList();
        }

        public async Task<object> InvokeAsync(MethodInfo method, object[] arguments)
        {
            int index;
            lock (random)
            {
                index = random.Next(invokers.Count);
            }
            return await invokers[index].InvokeAsync(method, arguments);
        }
    }
}
