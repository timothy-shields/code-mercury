using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using CodeMercury.Json;

namespace CodeMercury.Zmq
{
    public static class SocketHelper
    {
        public static SendStatus SendMessage(this Socket socket, JMessage message)
        {
            return socket.Send(JsonHelper.ToJson(message).ToString(), Encoding.ASCII);
        }

        public static SendStatus SendMessage(this Socket socket, JMessage message, params SendRecvOpt[] flags)
        {
            return socket.Send(JsonHelper.ToJson(message).ToString(), Encoding.ASCII, flags);
        }

        public static JMessage RecvMessage(this Socket socket)
        {
            return JsonHelper.ToObject<JMessage>(socket.Recv(Encoding.ASCII));
        }

        public static JMessage RecvMessage(this Socket socket, TimeSpan timeout)
        {
            return JsonHelper.ToObject<JMessage>(socket.Recv(Encoding.ASCII, (int)timeout.TotalMilliseconds));
        }

        public static JMessage RecvMessage(this Socket socket, params SendRecvOpt[] flags)
        {
            return JsonHelper.ToObject<JMessage>(socket.Recv(Encoding.ASCII, flags));
        }
    }
}
