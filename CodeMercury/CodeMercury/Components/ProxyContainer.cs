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
    public class ProxyContainer : IProxyContainer, IProxyResolver
    {
        private readonly IProxyActivator proxyActivator;
        private readonly IInvoker invoker;

        private readonly Dictionary<Guid, Type> serviceTypes = new Dictionary<Guid, Type>();

        public ProxyContainer(IProxyActivator proxyActivator, IInvoker invoker)
        {
            this.proxyActivator = proxyActivator;
            this.invoker = invoker;
        }

        public void Register(Guid serviceId, Type serviceType)
        {
            if (!serviceType.IsInterface)
            {
                throw new CodeMercuryBugException();
            }
            serviceTypes.Add(serviceId, serviceType);
        }

        public IProxy Resolve(Guid serviceId)
        {
            var proxyType = serviceTypes[serviceId];
            var proxyInvoker = new ProxyInvoker(invoker, serviceId);
            var proxy = proxyActivator.Create(proxyType, proxyInvoker);
            return proxy;
        }

        public void Release(Guid serviceId)
        {
            serviceTypes.Remove(serviceId);
        }
    }
}
