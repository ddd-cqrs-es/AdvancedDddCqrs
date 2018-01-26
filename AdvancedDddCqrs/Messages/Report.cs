using System;

namespace AdvancedDddCqrs.Messages
{
    public class Report : OrderMessage
    {
        public Report(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}
