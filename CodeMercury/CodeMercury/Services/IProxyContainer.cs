using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    /// <summary>
    /// Stores references to remote service types.
    /// </summary>
    public interface IProxyContainer
    {
        /// <summary>
        /// Registers a remote service type.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="serviceType">The service interface type.</param>
        void Register(Guid serviceId, Type serviceType);

        /// <summary>
        /// Unregisters a remote service type.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        void Release(Guid serviceId);
    }
}
