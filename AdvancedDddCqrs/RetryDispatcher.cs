using System;
using System.Threading;

namespace AdvancedDddCqrs
{
    public static class RetryDispatcher
    {
        public static RetryDispatcher<T> Wrap<T>(IHandler<T> handler)
        {
            return new RetryDispatcher<T>(handler);
        }
    }

    public class RetryDispatcher<T> : IHandler<T>
    {
        private readonly IHandler<T> _handler;

        public RetryDispatcher(IHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            _handler = handler;
        }

        public bool Handle(T message)
        {
            while (_handler.Handle(message) == false)
            {
                Thread.Sleep(1);
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("RetryDispatcher({0})", _handler);
        }
    }
}
