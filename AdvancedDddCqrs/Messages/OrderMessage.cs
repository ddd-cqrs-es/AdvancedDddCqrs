using System;

namespace AdvancedDddCqrs.Messages
{
    public class OrderMessage : IMessage, IHaveTTL
    {
        private DateTime? _expiry;

        protected OrderMessage(Order order, Guid correlationId, Guid? causationId = null)
        {
            MessageId = Guid.NewGuid();
            CorrelationId = correlationId;
            if (causationId.HasValue)
            {
                CausationId = causationId;
            }

            Order = order;
        }

        public Order Order { get; }

        public bool HasExpired()
        {
            return DateTime.UtcNow > _expiry;
        }

        public void SetExpiry(TimeSpan duration)
        {
            if (_expiry == null)
            {
                _expiry = DateTime.UtcNow.Add(duration);
            }
        }

        public Guid MessageId { get; }

        public Guid CorrelationId { get; }

        public Guid? CausationId { get; }
    }
}
