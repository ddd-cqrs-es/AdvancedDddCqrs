using System;

namespace AdvancedDddCqrs.Messages
{
    public class Completed : OrderMessage
    {
        public Completed(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}
