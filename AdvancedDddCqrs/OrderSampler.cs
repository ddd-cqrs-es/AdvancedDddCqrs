using System;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class OrderSampler : IHandler<OrderTaken>
    {
        private readonly ITopicDispatcher _topicDispatcher;
        private bool _finished;

        public OrderSampler(ITopicDispatcher topicDispatcher)
        {
            if (topicDispatcher == null)
            {
                throw new ArgumentNullException("topicDispatcher");
            }

            _topicDispatcher = topicDispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            if (_finished)
            {
                return true;
            }

            _topicDispatcher.Subscribe(message.CorrelationId.ToString(), new Printer());
            _finished = true;
            return true;
        }
    }
}
