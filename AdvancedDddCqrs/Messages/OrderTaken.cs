using System;

namespace AdvancedDddCqrs.Messages
{
    public class OrderTaken : OrderMessage
    {
        public OrderTaken(Order order, Guid correlationId, Guid? causationId)
            : base(order, correlationId, causationId)
        {
        }

        //public OrderTaken Clone()
        //{
        //    var clone = new OrderTaken(Order.Clone());
        //    clone._expiry = _expiry;
        //    return clone;
        //}
    }
}
