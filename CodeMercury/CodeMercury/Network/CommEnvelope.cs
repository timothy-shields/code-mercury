using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeMercury.Json;

namespace CodeMercury.Network
{
    public class CommEnvelope
    {
        public BoolString Sender { get; set; }
        public BoolString Recipient { get; set; }

        /// <summary>
        /// Purely for debugging
        /// </summary>
        public List<BoolString> Trace { get; set; }

        public JMessage Message { get; set; }

        public static CommEnvelope Create(BoolString recipient, JMessage message)
        {
            return new CommEnvelope
            {
                Recipient = recipient,
                Message = message
            };
        }
    }
}
