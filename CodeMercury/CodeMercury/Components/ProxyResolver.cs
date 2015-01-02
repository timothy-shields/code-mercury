using CodeMercury.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// Stores references to remote service types and resolves corresponding proxy instances.
    /// </summary>
    public class ProxyResolver : IProxyResolver
    {
        private readonly IProxyActivator proxyActivator;
        private readonly IInvoker invoker;

        public ProxyResolver(IProxyActivator proxyActivator, IInvoker invoker)
        {
            this.proxyActivator = proxyActivator;
            this.invoker = invoker;
        }

        public object Resolve(Guid serviceId, Type serviceType)
        {
            var proxyInvoker = new ProxyInvoker(invoker, serviceId);
            var proxy = proxyActivator.Create(serviceType, proxyInvoker);
            return proxy;
        }
    }
}
