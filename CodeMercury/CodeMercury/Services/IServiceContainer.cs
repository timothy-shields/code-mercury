using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    /// <summary>
    /// Stores local service instances.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Registers a local service instance.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="serviceInstance">The service instance.</param>
        void Register(Guid serviceId, object serviceInstance);

        /// <summary>
        /// Unregisters a local service instance.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        void Release(Guid serviceId);
    }
}
