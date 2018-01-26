using System;
using System.Collections.Concurrent;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Cashier : IHandler<QueueOrderForPayment>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly ConcurrentDictionary<Guid, OrderMessage> _ordersToBePaid = new ConcurrentDictionary<Guid, OrderMessage>();

        public Cashier(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _dispatcher = dispatcher;
        }

        public bool Handle(QueueOrderForPayment message)
        {
            _ordersToBePaid.TryAdd(message.Order.Id, message);
            return true;
        }

        public bool TryPay(Guid orderId)
        {
            OrderMessage orderMessage;
            if (_ordersToBePaid.TryGetValue(orderId, out orderMessage))
            {
                Order orderToPay = orderMessage.Order;
                orderToPay.SettleBill();
                OrderMessage removedOrder;
                _ordersToBePaid.TryRemove(orderId, out removedOrder);
                _dispatcher.Publish(new Paid(orderToPay, orderMessage.CorrelationId, orderMessage.MessageId));

                ConsoleColor origColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("$");
                Console.ForegroundColor = origColour;

                return true;
            }

            return false;
        }
    }
}
