using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury.Zmq
{
    /// <summary>
    /// A messaging endpoint: a place where messages are first sent or finally received.
    /// </summary>
    public class ZmqEndpoint
    {
        /// <summary>
        /// An address identifying a socket.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// An identity describing an endpoint communicating
        /// via the socket at the associated address. The identity
        /// should uniquely identify the endpoint.
        /// </summary>
        public string Identity { get; set; }
    }
}
