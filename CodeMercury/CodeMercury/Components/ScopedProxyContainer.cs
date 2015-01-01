using CodeMercury.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// A ProxyContainer that releases all registered services when it is disposed.
    /// </summary>
    public class ScopedProxyContainer : IProxyContainer, IDisposable
    {
        private readonly IProxyContainer container;
        private readonly HashSet<Guid> serviceIds = new HashSet<Guid>();
        private readonly object sync = new object();
        private bool disposed = false;

        public ScopedProxyContainer(IProxyContainer container)
        {
            this.container = container;
        }

        public void Register(Guid serviceId, Type serviceType)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                serviceIds.Add(serviceId);
                container.Register(serviceId, serviceType);
            }
        }

        public void Release(Guid serviceId)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                container.Release(serviceId);
                serviceIds.Remove(serviceId);
            }
        }

        public void Dispose()
        {
            lock (sync)
            {
                if (disposed)
                {
                    return;
                }
                foreach (var serviceId in serviceIds)
                {
                    container.Release(serviceId);
                }
                serviceIds.Clear();
                disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("ScopedProxyContainer");
            }
        }
    }
}
