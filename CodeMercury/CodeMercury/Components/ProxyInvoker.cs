using CodeMercury.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// A decorator for an <see cref="IInvoker"/> that replaces the Object
    /// of the <see cref="Invocation"/> with a <see cref="ServiceArgument"/>.
    /// </summary>
    public class ProxyInvoker : IInvoker
    {
        private readonly IInvoker invoker;
        private readonly Guid serviceId;

        public ProxyInvoker(IInvoker invoker, Guid serviceId)
        {
            this.invoker = invoker;
            this.serviceId = serviceId;
        }

        public async Task<Argument> InvokeAsync(Invocation invocation)
        {
            return await invoker.InvokeAsync(
                new Invocation(
                    new ServiceArgument(serviceId),
                    invocation.Method,
                    invocation.Arguments));
        }
    }
}
