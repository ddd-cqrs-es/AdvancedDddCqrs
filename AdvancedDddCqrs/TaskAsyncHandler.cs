using System;
using System.Threading.Tasks;

namespace AdvancedDddCqrs
{
    public class TaskAsyncHandler : IOrderHandler
    {
        private readonly IOrderHandler _handler;

        public TaskAsyncHandler(IOrderHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            _handler = handler;
        }

        public bool Handle(Order order)
        {
            Task.Factory.StartNew(() => _handler.Handle(order));

            return true;
        }
    }
}
