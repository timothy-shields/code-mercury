using CodeMercury.Domain.Models;
using CodeMercury.Services;
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

        public async Task<Argument> InvokeAsync(Invocation invocation)
        {
            int index;
            lock (random)
            {
                index = random.Next(invokers.Count);
            }
            var invoker = invokers[index];
            return await invoker.InvokeAsync(invocation);
        }
    }
}
