using System;
using System.Collections.Generic;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class OrderFulfillmentCoordinator : IHandler<OrderTaken>, IHandler<IMessage>, IHandler<Completed>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly Dictionary<Guid, IProcessManager> _processes = new Dictionary<Guid, IProcessManager>();

        public OrderFulfillmentCoordinator(ITopicDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool Handle(Completed message)
        {
            IProcessManager orderFulfillment;
            if (!_processes.TryGetValue(message.CorrelationId, out orderFulfillment))
            {
                _dispatcher.Unsubscribe<IMessage>(message.CorrelationId.ToString(), this);
                _processes.Remove(message.CorrelationId);
            }

            return true;
        }

        public bool Handle(IMessage message)
        {
            IProcessManager orderFulfillment;
            if (_processes.TryGetValue(message.CorrelationId, out orderFulfillment))
            {
                ((dynamic)orderFulfillment).Handle((dynamic)message);
            }

            return true;
        }

        public bool Handle(OrderTaken message)
        {
            AddNewOrderFulfillment(message);
            return true;
        }

        private void AddNewOrderFulfillment(OrderTaken message)
        {
            IProcessManager orderFulfillment;
            if (!_processes.TryGetValue(message.CorrelationId, out orderFulfillment))
            {
                if (message.Order.DodgeyCustomer)
                {
                    orderFulfillment = new OrderFulfillmentForDodgeyCustomer(_dispatcher, message);
                }
                else
                {
                    orderFulfillment = new OrderFulfillment(_dispatcher, message);
                }

                _processes.Add(message.CorrelationId, orderFulfillment);
                _dispatcher.Subscribe<IMessage>(message.CorrelationId.ToString(), this);
            }
        }
    }
}
