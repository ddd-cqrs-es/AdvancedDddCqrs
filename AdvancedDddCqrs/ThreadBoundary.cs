using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AdvancedDddCqrs
{
    public class ThreadBoundary<T> : IHandler<T>, IDisposable, IReportingThreadBoundary
    {
        private readonly IHandler<T> _handler;
        private BlockingCollection<T> _queue = new BlockingCollection<T>();

        public ThreadBoundary(IHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            _handler = handler;

            Task.Factory.StartNew(() =>
            {
                foreach (T order in _queue.GetConsumingEnumerable())
                {
                    handler.Handle(order);
                }
            });
        }

        public int QueueLength => _queue.Count;

        public void Dispose()
        {
            if (_queue != null)
            {
                _queue.CompleteAdding();
                _queue = null;
            }
        }

        public bool Handle(T message)
        {
            _queue.Add(message);

            return true;
        }

        public string GetName()
        {
            return ToString();
        }

        public int GetQueueLength()
        {
            return QueueLength;
        }

        public override string ToString()
        {
            return string.Format("ThreadBoundary({0})", _handler);
        }
    }

    public interface IReportingThreadBoundary
    {
        string GetName();

        int GetQueueLength();
    }
}
