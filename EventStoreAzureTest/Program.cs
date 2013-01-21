using EventStore;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventStoreAzureTest
{
    class Program
    {
        const int AmountOfRoots = 100 * 1000;
        const int AmountOfCommitsPerRoot = 10;
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            using (var eventStore = WireupEventStore())
            {
                Console.Write("Inserting {0} AggregateRoots with {1} commits each", AmountOfRoots, AmountOfCommitsPerRoot);
                stopWatch.Start();
                byte[] payload = new byte[80];
                var resetEvents = new ConcurrentBag<ManualResetEventSlim>();

                for (int i = 0; i < AmountOfRoots; ++i)
                {
                    if (i % (AmountOfRoots / 100) == 0)
                    {
                        Thread.Sleep(500);
                        Console.Write(".");
                    }

                    var resetEvent = new ManualResetEventSlim();
                    resetEvents.Add(resetEvent);

                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        var currentRoot = Guid.NewGuid();
                        for (int commits = 0; commits < AmountOfCommitsPerRoot; ++commits)
                        {
                            using (var stream = eventStore.OpenStream(currentRoot, 0, int.MaxValue))
                            {
                                stream.Add(new EventMessage() { Body = payload });
                                stream.CommitChanges(Guid.NewGuid());
                            }
                        }

                        ((ManualResetEventSlim)x).Set();
                    }, resetEvent);
                }
                while (!resetEvents.All(x => x.IsSet))
                {
                    Thread.Sleep(100);
                }
                stopWatch.Stop();
                Console.WriteLine("done in {0} ms.", stopWatch.ElapsedMilliseconds);
                Console.ReadKey();
            }
        }

        private static IStoreEvents WireupEventStore()
        {
            var hook = new ProbabilisticPipelineHook();

            var es = Wireup.Init()
               .LogToOutputWindow()
               .UsingAzureTablesPersistence("Tables")
                    .InitializeStorageEngine()
               .UsingBinarySerialization()
               .HookIntoPipelineUsing(hook)
               .Build();


            hook.Persistence = es.Advanced;

            return es;
        }
    }
}
