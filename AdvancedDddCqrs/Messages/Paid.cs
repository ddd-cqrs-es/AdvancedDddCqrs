using System;

namespace AdvancedDddCqrs.Messages
{
    public class Paid : OrderMessage
    {
        public Paid(Order order, Guid correlationId, Guid? causationId) : base(order, correlationId, causationId)
        {
        }
    }
}
