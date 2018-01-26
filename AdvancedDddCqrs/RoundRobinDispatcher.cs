using System;
using System.Collections.Generic;

namespace AdvancedDddCqrs
{
    public class RoundRobinDispatcher : IOrderHandler
    {
        private readonly Queue<IOrderHandler> _handlers = new Queue<IOrderHandler>();

        public RoundRobinDispatcher(IEnumerable<IOrderHandler> handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            foreach (IOrderHandler orderHandler in handlers)
            {
                _handlers.Enqueue(orderHandler);
            }
        }

        public bool Handle(Order order)
        {
            IOrderHandler handler = _handlers.Dequeue();
            handler.Handle(order);
            _handlers.Enqueue(handler);

            return true;
        }
    }
}
