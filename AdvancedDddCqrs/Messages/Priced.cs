using System;

namespace AdvancedDddCqrs.Messages
{
    public class Priced : OrderMessage
    {
        public Priced(Order order, Guid correlationId, Guid? causationId)
            : base(order, correlationId, causationId)
        {
        }
    }
}
