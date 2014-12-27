using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents the proxy end of a service-proxy link.
    /// The sender of a <see cref="ProxyArgument"/> is on the same machine as the service.
    /// The recipient of a <see cref="ProxyArgument"/> is on a different machine than the service.
    /// </summary>
    public class ProxyArgument : Argument
    {
        /// <summary>
        /// The ID of the service being proxied.
        /// </summary>
        public Guid ServiceId { get; private set; }

        public ProxyArgument(Guid serviceId)
        {
            this.ServiceId = serviceId;
        }

        public override string ToString()
        {
            return string.Format("Proxy({0})", ServiceId);
        }
    }
}
