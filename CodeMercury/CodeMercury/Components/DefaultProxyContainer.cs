using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class DefaultProxyContainer : IProxyContainer, IProxyResolver
    {
        private readonly IProxyActivator proxyActivator;
        private readonly IProxyableContainer proxyableRepository;
        private readonly Uri requesterUri;
        private readonly Uri serverUri;

        private readonly Dictionary<Guid, Type> proxyTypes = new Dictionary<Guid, Type>();

        public DefaultProxyContainer(
            IProxyActivator proxyActivator,
            IProxyableContainer proxyableRepository,
            Uri requesterUri,
            Uri serverUri)
        {
            this.proxyActivator = proxyActivator;
            this.proxyableRepository = proxyableRepository;
            this.requesterUri = requesterUri;
            this.serverUri = serverUri;
        }

        public void Register(Guid id, Type proxyType)
        {
            proxyTypes.Add(id, proxyType);
        }

        public IProxy Resolve(Guid id)
        {
            var proxyType = proxyTypes[id];
            var invoker = new HttpInvoker(requesterUri, serverUri, proxyableRepository);
            var proxy = proxyActivator.Create(proxyType, invoker);
            return proxy;
        }
    }
}
