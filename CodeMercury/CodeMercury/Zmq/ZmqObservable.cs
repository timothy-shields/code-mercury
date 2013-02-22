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
    /// </summary>
    public class ZmqObservable : IObservable<ZmqEnvelope>
    {
        public string Identity { get; private set; }
        public string Address { get; private set; }

        private CancellationToken cancellationToken;

        private Subject<ZmqEnvelope> subject;
        private Thread thread;

        private ZmqObservable(string identity, string address, CancellationToken cancellationToken)
        {
            this.Identity = identity;
            this.Address = address;
            this.cancellationToken = cancellationToken;

            this.subject = new Subject<ZmqEnvelope>();

            this.thread = new Thread(Run);
            this.thread.Start();
        }

        public static ZmqObservable Create(string identity, string address, CancellationToken cancellationToken)
        {
            return new ZmqObservable(identity, address, cancellationToken);
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
                        subject.OnNext(new ZmqEnvelope
                        {
                            Sender = new ZmqEndpoint
                            {
                                Address = this.Address,
                                Identity = sender
                            },
                            Recipient = new ZmqEndpoint
                            {
                                Address = null,
                                Identity = this.Identity
                            },
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
