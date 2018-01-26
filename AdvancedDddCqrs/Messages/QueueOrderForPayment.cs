using System;

namespace AdvancedDddCqrs.Messages
{
    public class QueueOrderForPayment : OrderMessage
    {
        public QueueOrderForPayment(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}
