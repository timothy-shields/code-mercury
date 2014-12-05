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
using CodeMercury.Information;

namespace CodeMercury.Network
{
    public class CommNode : ISubject<CommEnvelope>
    {
        public BoolString Identity { get; private set; }
        public string Address { get; private set; }

        private Dictionary<BoolString, string> neighbors;
        private InformationSet informationSet;

        private Subject<CommEnvelope> subject;

        private CancellationToken cancellationToken;
        private ZmqNode zmqNode;
        private Random random = new Random();

        public CommNode(BoolString identity, string address, List<Tuple<BoolString, string>> connectTo, CancellationToken cancellationToken)
        {
            this.Identity = identity;
            this.Address = address;

            this.neighbors = connectTo.ToDictionary(x => x.Item1, x => x.Item2);

            this.subject = new Subject<CommEnvelope>();

            this.cancellationToken = cancellationToken;
            this.zmqNode = new ZmqNode(CommHelper.MercuryToZmq(identity), address, connectTo.Select(x => x.Item2), cancellationToken);

            zmqNode.Subscribe(ZmqOnNext, ZmqOnError, ZmqOnCompleted, cancellationToken);
        }

        private void ZmqOnNext(ZmqEnvelope zmqEnvelope)
        {
            zmqEnvelope.Message.When<CommEnvelope>(mercuryEnvelope =>
            {
                mercuryEnvelope.Trace.Add(this.Identity);

                if (mercuryEnvelope.Recipient.Equals(this.Identity))
                {
                    if (CommDebug.PrintSendRecv)
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
            subject.OnError(e);
        }

        private void ZmqOnCompleted()
        {
            subject.OnCompleted();
            subject.Dispose();
        }


        private void Send(CommEnvelope mercuryEnvelope)
        {
            //Forward through the network toward the final recipient
            var neighborIdentity = RandomHelper.RandomItem(
                neighbors.Keys.MinBy(x => BoolString.HammingDistance(x, mercuryEnvelope.Recipient)));
            zmqNode.OnNext(ZmqEnvelope.Create(CommHelper.MercuryToZmq(neighborIdentity), JMessage.FromValue(mercuryEnvelope)));
        }

        public void OnNext(CommEnvelope mercuryEnvelope)
        {
            mercuryEnvelope.Sender = this.Identity;

            mercuryEnvelope.Trace = new List<BoolString> { this.Identity };

            if (CommDebug.PrintSendRecv)
                Console.WriteLine("mercury: [{0}] --send-> [{1}]", mercuryEnvelope.Sender, mercuryEnvelope.Recipient);

            Send(mercuryEnvelope);
        }

        public void OnError(System.Exception error)
        {
            zmqNode.OnError(error);
        }

        public void OnCompleted()
        {
            zmqNode.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<CommEnvelope> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
