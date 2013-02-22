using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZMQ;
using System.Text;
using Newtonsoft.Json.Linq;
using CodeMercury.Json;

namespace CodeMercury.Zmq
{
    /// <summary>
    /// Abstracted ZMQ Sender
    /// </summary>
    public class ZmqObserver : IObserver<ZmqEnvelope>
    {
        public string Identity { get; private set; }
        public IEnumerable<string> ConnectTo { get { return connectTo.Hide(); } }

        private List<string> connectTo;

        private BlockingCollection<ZmqEnvelope> queue;
        private Thread thread;

        private ZmqObserver(string identity, IEnumerable<string> connectTo)
        {
            this.Identity = identity;
            this.connectTo = connectTo.ToList();
            this.queue = new BlockingCollection<ZmqEnvelope>();
            this.thread = new Thread(Run);
            this.thread.Start();
        }

        public static ZmqObserver Create(string identity, IEnumerable<string> connectTo)
        {
            return new ZmqObserver(identity, connectTo);
        }

        public void OnNext(ZmqEnvelope value)
        {
            queue.Add(value);
        }

        public void OnError(System.Exception error)
        {
            queue.CompleteAdding();
        }

        public void OnCompleted()
        {
            queue.CompleteAdding();
        }

        private void Run()
        {
            using (var context = new Context())
            using (var router = context.Socket(SocketType.ROUTER))
            {
                router.Linger = 200;
                router.StringToIdentity(Identity, Encoding.Unicode);

                foreach (var otherAddress in connectTo)
                {
                    router.Connect(otherAddress);
                }

                foreach (var value in queue.GetConsumingEnumerable())
                {
                    while (true)
                    {
                        router.Send(value.Recipient.Identity, Encoding.Unicode, SendRecvOpt.SNDMORE);
                        if (SendStatus.TryAgain != router.Send(JMessage.Serialize(value.Message), SendRecvOpt.NOBLOCK))
                            break;
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }
}
