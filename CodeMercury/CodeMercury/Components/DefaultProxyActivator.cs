using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class DefaultProxyActivator : IProxyActivator
    {
        private readonly Dictionary<Type, Func<IInvoker, IProxy>> activators;

        public DefaultProxyActivator(IDictionary<Type, Func<IInvoker, IProxy>> activators)
        {
            this.activators = new Dictionary<Type, Func<IInvoker, IProxy>>(activators);
        }

        public IProxy Create(Type proxyType, IInvoker proxyInvoker)
        {
            var activator = activators[proxyType];
            var proxy = activator(proxyInvoker);
            return proxy;
        }
    }
}
