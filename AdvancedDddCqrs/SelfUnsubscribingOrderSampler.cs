using System;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class SelfUnsubscribingOrderSampler : IHandler<OrderTaken>
    {
        private readonly ITopicDispatcher _topicDispatcher;

        public SelfUnsubscribingOrderSampler(ITopicDispatcher topicDispatcher)
        {
            if (topicDispatcher == null)
            {
                throw new ArgumentNullException("topicDispatcher");
            }

            _topicDispatcher = topicDispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            _topicDispatcher.Unsubscribe(message.GetType().FullName, this);

            _topicDispatcher.Subscribe(message.CorrelationId.ToString(), new Printer());

            return true;
        }
    }
}
