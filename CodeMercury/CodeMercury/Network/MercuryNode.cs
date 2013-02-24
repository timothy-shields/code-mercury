using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading;
using CodeMercury.Zmq;
using CodeMercury.Models;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeMercury.Json;

namespace CodeMercury.Network
{
    public class MercuryNode : ISubject<MercuryEnvelope>
    {
        public BoolString Identity { get; private set; }
        public string Address { get; private set; }

        private Dictionary<BoolString, string> neighbors;

        private Subject<MercuryEnvelope> subject;

        private CancellationToken cancellationToken;
        private ZmqObservable zmqObservable;
        private ZmqObserver zmqObserver;
        private Random random = new Random();

        //private FunctionApplication functionApplication;
        //private FunctionResult functionResult;

        /// <summary>
        /// The set of identities to notify on the completion of the current FunctionApplication.
        /// </summary>
        //private HashSet<string> notify = new HashSet<string>();

        private MercuryNode(BoolString identity, string address, List<Tuple<BoolString, string>> connectTo, CancellationToken cancellationToken)
        {
            this.Identity = identity;
            this.Address = address;

            this.neighbors = connectTo.ToDictionary(x => x.Item1, x => x.Item2);

            this.subject = new Subject<MercuryEnvelope>();

            this.cancellationToken = cancellationToken;
            this.zmqObservable = ZmqObservable.Create(MercuryHelper.MercuryToZmq(identity), address, cancellationToken);
            this.zmqObserver = ZmqObserver.Create(MercuryHelper.MercuryToZmq(identity), connectTo.Select(x => x.Item2), cancellationToken);

            zmqObservable.Subscribe(ZmqOnNext, ZmqOnError, ZmqOnCompleted, cancellationToken);
        }

        public static MercuryNode Create(BoolString identity, string address, List<Tuple<BoolString, string>> connectTo, CancellationToken cancellationToken)
        {
            return new MercuryNode(identity, address, connectTo, cancellationToken);
        }

        private void ZmqOnNext(ZmqEnvelope zmqEnvelope)
        {
            zmqEnvelope.Message.When<MercuryEnvelope>(mercuryEnvelope =>
            {
                mercuryEnvelope.Trace.Add(this.Identity);

                if (mercuryEnvelope.Recipient.Equals(this.Identity))
                {
                    if (MercuryDebug.PrintSendRecv)
                    {
                        Console.WriteLine("mercury: [{0}] <-recv-- [{1}]", mercuryEnvelope.Recipient, mercuryEnvelope.Sender);
                        Console.WriteLine("trace: {0}", string.Join(" -> ", mercuryEnvelope.Trace.Select(x => "[{0}]".Format(x))));
                    }

                    subject.OnNext(mercuryEnvelope);
                }
                else
                {
                    Send(mercuryEnvelope);
                }
            });
        }

        private void ZmqOnError(System.Exception e)
        {
        }

        private void ZmqOnCompleted()
        {
            subject.OnCompleted();
            subject.Dispose();
        }


        private void Send(MercuryEnvelope mercuryEnvelope)
        {
            //Forward through the network toward the final recipient
            var neighborIdentity = RandomHelper.RandomItem(
                neighbors.Keys.MinBy(x => BoolString.HammingDistance(x, mercuryEnvelope.Recipient)));
            zmqObserver.OnNext(ZmqEnvelope.Create(MercuryHelper.MercuryToZmq(neighborIdentity), JMessage.FromValue(mercuryEnvelope)));
        }

        public void OnNext(MercuryEnvelope mercuryEnvelope)
        {
            mercuryEnvelope.Sender = this.Identity;

            mercuryEnvelope.Trace = new List<BoolString> { this.Identity };

            if (MercuryDebug.PrintSendRecv)
                Console.WriteLine("mercury: [{0}] --send-> [{1}]", mercuryEnvelope.Sender, mercuryEnvelope.Recipient);

            Send(mercuryEnvelope);
        }

        public void OnError(System.Exception error)
        {
            zmqObserver.OnError(error);
        }

        public void OnCompleted()
        {
            zmqObserver.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<MercuryEnvelope> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
