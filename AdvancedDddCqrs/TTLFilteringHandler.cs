using System;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public static class TTLFilteringHandler
    {
        public static TTLFilteringHandler<T> Wrap<T>(IHandler<T> handler)
        {
            return new TTLFilteringHandler<T>(handler);
        }
    }

    public class TTLFilteringHandler<T> : IHandler<T>
    {
        private readonly IHandler<T> _handler;

        public TTLFilteringHandler(IHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            _handler = handler;
        }

        public bool Handle(T message)
        {
            var hasTtl = message as IHaveTTL;
            if (hasTtl == null || hasTtl.HasExpired() == false)
            {
                return _handler.Handle(message);
            }

            ConsoleColor origColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("-");
            Console.ForegroundColor = origColour;
            return true;
        }

        public override string ToString()
        {
            return string.Format("TTLFilteringHandler({0})", _handler);
        }
    }
}
