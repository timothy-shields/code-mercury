using System;
using System.Reactive.Subjects;
using System.Threading;
using ZMQ;
using System.Text;
using CodeMercury.Json;

namespace CodeMercury.Zmq
{
    /// <summary>
    /// Abstracted ZMQ Receiver
    /// Want in the future to include support for different socket types (ROUTER, DEALER, PULL, SUB)
    /// </summary>
    public class ZmqObservable : IObservable<ZmqEnvelope>
    {
        public string Identity { get; private set; }
        public string Address { get; private set; }

        private CancellationToken cancellationToken;

        private Subject<ZmqEnvelope> subject;
        private Thread thread;

        public ZmqObservable(string identity, string address, CancellationToken cancellationToken)
        {
            this.Identity = identity;
            this.Address = address;
            this.cancellationToken = cancellationToken;

            this.subject = new Subject<ZmqEnvelope>();

            this.thread = new Thread(Run);
            this.thread.Start();
        }

        public IDisposable Subscribe(IObserver<ZmqEnvelope> observer)
        {
            return subject.Subscribe(observer);
        }

        private void Run()
        {
            try
            {
                using (var context = new Context())
                using (var router = context.Socket(SocketType.ROUTER))
                {
                    router.StringToIdentity(Identity, Encoding.Unicode);
                    router.Bind(Address);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var sender = router.Recv(Encoding.Unicode, 200);
                        if (sender == null)
                            continue;
                        var value = router.Recv();

                        if (ZmqDebug.PrintSendRecv)
                            Console.WriteLine("zmq: [{0}] <-recv-- [{1}]", this.Identity, sender);

                        subject.OnNext(new ZmqEnvelope
                        {
                            Sender = sender,
                            Recipient = this.Identity,
                            Message = JMessage.Deserialize(value)
                        });
                    }
                }
            }
            finally
            {
                subject.OnCompleted();
                subject.Dispose();
            }
        }
    }
}
