using System;

namespace AdvancedDddCqrs.Messages
{
    public class PriceFood : OrderMessage
    {
        public PriceFood(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}
