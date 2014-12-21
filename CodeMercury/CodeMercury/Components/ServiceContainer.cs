using CodeMercury.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// Stores local service instances.Resolves local service instances.
    /// </summary>
    public class ServiceContainer : IServiceContainer, IServiceResolver
    {
        private Dictionary<Guid, object> services = new Dictionary<Guid, object>();

        /// <summary>
        /// Registers a local service instance.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="serviceInstance">The service instance.</param>
        public void Register(Guid serviceId, object serviceInstance)
        {
            lock (services)
            {
                services.Add(serviceId, serviceInstance);
            }
        }

        /// <summary>
        /// Resolves a local service instance by ID.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <returns>The service instance.</returns>
        public object Resolve(Guid serviceId)
        {
            lock (services)
            {
                var service = services[serviceId];
                return service;
            }
        }

        public void Release(Guid serviceId)
        {
            lock (services)
            {
                services.Remove(serviceId);
            }
        }
    }
}
