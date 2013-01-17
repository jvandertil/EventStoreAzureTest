using EventStore;
using EventStore.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreAzureTest
{
    public class ProbabilisticPipelineHook : IPipelineHook
    {
        private readonly Random _random = new Random();
        public IPersistStreams Persistence;

        public void PostCommit(Commit committed)
        {
            // 1.0 = 100 %
            // 0.001 = 0.01 %
            if (_random.NextDouble() > 0.001)
                Persistence.MarkCommitAsDispatched(committed);
        }

        public bool PreCommit(Commit attempt)
        {
            return true;
        }

        public Commit Select(Commit committed)
        {
            return committed;
        }

        public void Dispose()
        {
            // No-op
        }
    }
}
