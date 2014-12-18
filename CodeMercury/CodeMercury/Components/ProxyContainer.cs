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
        private readonly IServiceContainer serviceContainer;
        private readonly Uri requesterUri;
        private readonly Uri serverUri;

        private readonly Dictionary<Guid, Type> serviceTypes = new Dictionary<Guid, Type>();

        public ProxyContainer(
            IProxyActivator proxyActivator,
            IServiceContainer serviceContainer,
            Uri requesterUri,
            Uri serverUri)
        {
            this.proxyActivator = proxyActivator;
            this.serviceContainer = serviceContainer;
            this.requesterUri = requesterUri;
            this.serverUri = serverUri;
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
            var invoker = new ProxyInvoker(new HttpInvoker(requesterUri, serverUri, serviceContainer), serviceId);
            var proxy = proxyActivator.Create(proxyType, invoker);
            return proxy;
        }
    }
}
