using EventStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventStoreAzureGetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var eventStore = WireupEventStore())
            {
                var stopwatch = new Stopwatch();
                Console.Write("Retrieving undispatched commits...");
                stopwatch.Start();
                var undispatched = eventStore.Advanced.GetUndispatchedCommits().ToArray();
                stopwatch.Stop();
                Console.WriteLine("retrieved {0} commits in {1} ms.", undispatched.Length, stopwatch.ElapsedMilliseconds);
                Console.ReadKey();
            }

        }
        private static IStoreEvents WireupEventStore()
        {
            var es = Wireup.Init()
               .LogToOutputWindow()
               .UsingAzureTablesPersistence("Tables")
               .UsingBinarySerialization()
               .Build();

            return es;
        }
    }
}
