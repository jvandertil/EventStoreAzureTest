using EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreAzureParallelGet
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var eventStore = WireupEventStore()) {
                //var allARs = eventStore.Advanced.GetStreamsToSnapshot(1).Select(x => x.StreamId).ToArray();
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
