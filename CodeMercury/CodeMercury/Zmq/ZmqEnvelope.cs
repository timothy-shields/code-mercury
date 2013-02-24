using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeMercury.Json;

namespace CodeMercury.Zmq
{
    public class ZmqEnvelope
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public JMessage Message { get; set; }

        public static ZmqEnvelope Create(string recipient, JMessage message)
        {
            return new ZmqEnvelope
            {
                Recipient = recipient,
                Message = message
            };
        }
    }
}
