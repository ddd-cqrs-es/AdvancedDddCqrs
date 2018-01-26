using System;

namespace AdvancedDddCqrs
{
    public static class RandomMessageFail
    {
        public static RandomMessageFail<T> Wrap<T>(IHandler<T> handler)
        {
            return new RandomMessageFail<T>(handler);
        }
    }

    public class RandomMessageFail<T> : IHandler<T>
    {
        private readonly IHandler<T> _handler;
        private readonly Random _random = new Random();

        public RandomMessageFail(IHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            _handler = handler;
        }

        public bool Handle(T message)
        {
            if (_random.NextDouble() >= 0.5)
            {
                _handler.Handle(message);
            }

            return true;
        }
    }
}
