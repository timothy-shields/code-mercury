using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeMercury.Json;

namespace CodeMercury.Zmq
{
    /// <summary>
    /// A message with "from" and "to" endpoint information.
    /// </summary>
    public class ZmqEnvelope
    {
        public ZmqEndpoint Sender { get; set; }
        public ZmqEndpoint Recipient { get; set; }
        public JMessage Message { get; set; }

        /// <summary>
        /// Create an envelope by specifying a recipient and a message.
        /// The sender endpoint and recipient address should be inserted
        /// by the ZmqObserver performing the send operation.
        /// </summary>
        public static ZmqEnvelope FromRecipientAndMessage(string recipientIdentity, JMessage message)
        {
            return new ZmqEnvelope
            {
                Recipient = new ZmqEndpoint
                {
                    Identity = recipientIdentity
                },
                Message = message
            };
        }
    }
}
