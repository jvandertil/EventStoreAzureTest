using EventStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreAzureTest
{
    class Program
    {
        const int AmountOfRoots = 1 * 1000 * 1000;
        const int AmountOfCommitsPerRoot = 10;
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            using (var eventStore = WireupEventStore())
            {
                Console.Write("Inserting {0} AggregateRoots with {1} commits each", AmountOfRoots, AmountOfCommitsPerRoot);
                stopWatch.Start();
                byte[] payload = new byte[80];

                Guid currentRoot = Guid.Empty;
                for (int i = 0; i < AmountOfRoots; ++i)
                {
                    if (i % (AmountOfRoots / 100) == 0)
                        Console.Write(".");

                    currentRoot = Guid.NewGuid();
                    for (int commits = 0; commits < AmountOfCommitsPerRoot; ++commits)
                    {
                        using (var stream = eventStore.OpenStream(currentRoot, 0, int.MaxValue))
                        {
                            stream.Add(new EventMessage() { Body = payload });
                            stream.CommitChanges(Guid.NewGuid());
                        }
                    }
                }
                stopWatch.Stop();
                Console.WriteLine("done in {0} ms.", stopWatch.ElapsedMilliseconds);
            }

            using (var eventStore = WireupEventStore())
            {
                Console.Write("Retrieving all AggregateRoots...");
                stopWatch.Restart();
                // No events will be dispatched, so this will get the whole DB in a table scan.
                var allEvents = eventStore.Advanced.GetUndispatchedCommits().ToArray();
                stopWatch.Stop();
                Console.WriteLine("done in {0} ms.", stopWatch.ElapsedMilliseconds);
            }
        }

        private static IStoreEvents WireupEventStore()
        {
            var es = Wireup.Init()
               .LogToOutputWindow()
               .UsingAzureTablesPersistence("Tables")
                    .InitializeStorageEngine()
               .UsingBinarySerialization()
               .Build();

            return es;
        }
    }
}
