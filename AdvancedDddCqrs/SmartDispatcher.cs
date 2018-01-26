using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedDddCqrs
{
    public static class SmartDispatcher
    {
        public static SmartDispatcher<T> Wrap<T>(IEnumerable<ThreadBoundary<T>> handlers, int maxQueueLength)
        {
            return new SmartDispatcher<T>(handlers, maxQueueLength);
        }
    }

    public class SmartDispatcher<T> : IHandler<T>
    {
        private readonly IEnumerable<ThreadBoundary<T>> _handlers;
        private readonly int _maxQueueLength;

        public SmartDispatcher(IEnumerable<ThreadBoundary<T>> handlers, int maxQueueLength)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            _handlers = handlers;
            _maxQueueLength = maxQueueLength;
        }

        public bool Handle(T message)
        {
            ThreadBoundary<T> next = GetNextHandler();

            if (next == null)
            {
                ////Console.WriteLine(string.Format("Pushing back order {0}", order.Id));
                return false;
            }

            next.Handle(message);
            return true;
        }

        public override string ToString()
        {
            ThreadBoundary<T> firstHandler = _handlers.FirstOrDefault();
            return string.Format(
                "SmartDispatcher({0})",
                firstHandler != null
                    ? firstHandler.ToString()
                    : string.Format("IHandler<{0}>", typeof(T)));
        }

        private ThreadBoundary<T> GetNextHandler()
        {
            return _handlers.Where(x => x.QueueLength < _maxQueueLength)
                            .OrderBy(x => x.QueueLength)
                            .FirstOrDefault();
        }
    }
}
