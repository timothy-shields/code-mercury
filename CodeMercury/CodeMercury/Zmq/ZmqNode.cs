using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Subjects;
using System.Threading;

namespace CodeMercury.Zmq
{
    public class ZmqNode : ISubject<ZmqEnvelope>
    {
        private ZmqObserver observer;
        private ZmqObservable observable;

        public ZmqNode(string identity, string address, IEnumerable<string> connectTo, CancellationToken cancellationToken)
        {
            this.observable = new ZmqObservable(identity, address, cancellationToken);
            this.observer = new ZmqObserver(identity, connectTo, cancellationToken);
        }

        public void OnNext(ZmqEnvelope value)
        {
            observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public void OnCompleted()
        {
            observer.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<ZmqEnvelope> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}
