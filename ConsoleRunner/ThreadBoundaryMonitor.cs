using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AdvancedDddCqrs;

namespace ConsoleRunner
{
    internal class ThreadBoundaryMonitor
    {
        private readonly IList<IReportingThreadBoundary> _cache = new List<IReportingThreadBoundary>();

        public ThreadBoundary<TEvent> Wrap<TEvent>(IHandler<TEvent> handler)
        {
            var tb = new ThreadBoundary<TEvent>(handler);
            _cache.Add(tb);
            return tb;
        }

        public void Start()
        {
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Console.Clear();
                        IEnumerable<string> tbs = _cache.Select(x => string.Format("{0}    : {1}", x.GetQueueLength(), x.GetName()));
                        foreach (string tb in tbs)
                        {
                            Console.WriteLine(tb);
                        }

                        Thread.Sleep(5000);
                    }
                },
                TaskCreationOptions.AttachedToParent);
        }
    }
}
