using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using CodeMercury;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive;
using CodeMercury.Zmq;
using ZMQ;
using CodeMercury.Graphs;
using CodeMercury.Json;
using CodeMercury.Network;

namespace CodeMercury.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Reactive.PlatformServices.EnlightenmentProvider.EnsureLoaded();

            var graph = GraphHelper.HammingGraph(6, 1);

            var addresses = graph.Keys
                .Zip(Enumerable.Range(5555, 9999).Select(i => "tcp://127.0.0.1:{0}".Format(i)), (identity, address) => new { identity, address })
                .ToDictionary(x => x.identity, x => x.address);

            var cancellationSource = new CancellationTokenSource();

            var mercuryNodes = graph.Keys
                .Select(identity =>
                {
                    var address = addresses[identity];
                    var connectTo = graph[identity].Select(neighbor => Tuple.Create(neighbor, addresses[neighbor])).ToList();
                    return new CommNode(identity, address, connectTo, cancellationSource.Token);
                })
                .ToDictionary(x => x.Identity);

            int sent = 0;
            int received = 0;
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(i => Console.WriteLine("sent={0} in-transit/dropped={1} received={2}", sent, sent - received, received), cancellationSource.Token);

            mercuryNodes[new BoolString("111111")].Subscribe(e => Interlocked.Increment(ref received));
            mercuryNodes[new BoolString("000111")].Subscribe(e => Interlocked.Increment(ref received));

            Random random = new Random();
            byte[] buffer = new byte[1024];

            //wait a little bit for nodes to connect to each other
            Thread.Sleep(1000);

            for (int i = 0; i < 10000; i++)
            {
                random.NextBytes(buffer);
                Interlocked.Increment(ref sent);
                mercuryNodes[new BoolString("000000")].OnNext(CommEnvelope.Create(new BoolString("111111"), JMessage.FromValue(buffer)));
                Interlocked.Increment(ref sent);
                mercuryNodes[new BoolString("111000")].OnNext(CommEnvelope.Create(new BoolString("000111"), JMessage.FromValue(buffer)));
            }

            Console.ReadLine();
            cancellationSource.Cancel();
        }
    }
}
