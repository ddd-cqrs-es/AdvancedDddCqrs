using System.Collections.Generic;
using System.Linq;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Multiplexer<T> : IHandler<T> where T : class, IMessage
    {
        private readonly IList<IHandler<T>> _handlers = new List<IHandler<T>>();

        public Multiplexer(IEnumerable<IHandler<T>> handlers)
        {
            _handlers = handlers.ToList();
        }

        public bool Handle(T message)
        {
            foreach (IHandler<T> handler in _handlers)
            {
                ////Console.WriteLine("Multiplexing {0} to {1}", message, handler);
                handler.Handle(message);
            }

            return true;
        }

        public void AddHandler(IHandler<T> newHandler)
        {
            _handlers.Add(newHandler);
        }

        public Multiplexer<T> Clone()
        {
            var clone = new Multiplexer<T>(_handlers);
            return clone;
        }

        public void RemoveHandler<TMsg>(IHandler<TMsg> handlerToRemove) where TMsg : class, IMessage
        {
            NarrowingHandler<IMessage, TMsg> narrowingHandler = _handlers.OfType<NarrowingHandler<IMessage, TMsg>>()
                                            .SingleOrDefault(x => x.Handler == handlerToRemove);

            if (narrowingHandler != null)
            {
                _handlers.Remove((IHandler<T>)narrowingHandler);
            }
        }

        public override string ToString()
        {
            IHandler<T> firstHandler = _handlers.FirstOrDefault();
            return string.Format(
                "Multiplexer({0})",
                firstHandler != null
                    ? firstHandler.ToString()
                    : string.Format("IHandler<{0}>", typeof(T)));
        }
    }
}
