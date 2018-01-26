using System;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class OrderFulfillmentForDodgeyCustomer : IProcessManager,
        IHandler<Cooked>,
        IHandler<Priced>,
        IHandler<Paid>,
        IHandler<Completed>,
        IHandler<IMessage>
    {
        private readonly ITopicDispatcher _dispatcher;

        public OrderFulfillmentForDodgeyCustomer(ITopicDispatcher dispatcher, OrderTaken initiatingMessage)
        {
            _dispatcher = dispatcher;

            _dispatcher.Publish(new PriceFood(initiatingMessage.Order,
                initiatingMessage.CorrelationId,
                initiatingMessage.MessageId));
        }

        public bool Handle(Completed message)
        {
            ConsoleColor origColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("!");
            Console.ForegroundColor = origColour;

            //Unsubscribe
            _dispatcher.Unsubscribe<Cooked>(message.CorrelationId.ToString(), this);
            _dispatcher.Unsubscribe<Priced>(message.CorrelationId.ToString(), this);
            _dispatcher.Unsubscribe<Paid>(message.CorrelationId.ToString(), this);
            _dispatcher.Unsubscribe<Completed>(message.CorrelationId.ToString(), this);
            return true;
        }

        public bool Handle(Cooked message)
        {
            _dispatcher.Publish(new Report(message.Order,
                message.CorrelationId,
                message.MessageId));
            return true;
        }

        public bool Handle(IMessage message)
        {
            return true;
        }

        public bool Handle(Paid message)
        {
            _dispatcher.Publish(new CookFood(message.Order,
                message.CorrelationId,
                message.MessageId));
            return true;
        }

        public bool Handle(Priced message)
        {
            _dispatcher.Publish(new QueueOrderForPayment(message.Order,
                message.CorrelationId,
                message.MessageId));
            return true;
        }
    }
}
