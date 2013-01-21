using EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStorePurger
{
    class Program
    {
        static void Main(string[] args)
        {
            WireupEventStore();
        }

        private static IStoreEvents WireupEventStore()
        {
            var es = Wireup.Init()
               .LogToOutputWindow()
               .UsingAzureTablesPersistence("Tables")
                    .InitializeStorageEngine()
               .UsingBinarySerialization()
               .Build();

            es.Advanced.Purge();

            return es;
        }
    }
}
