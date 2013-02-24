using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury.Network
{
    public static class MercuryHelper
    {
        public static string MercuryToZmq(BoolString mercuryIdentity)
        {
            return "b:" + mercuryIdentity.ToString();
        }

        public static BoolString ZmqToMercury(string zmqIdentity)
        {
            return new BoolString(zmqIdentity.Substring("b:".Length));
        }
    }
}
