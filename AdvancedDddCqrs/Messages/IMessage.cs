using System;

namespace AdvancedDddCqrs.Messages
{
    public interface IMessage
    {
        Guid MessageId { get; }

        Guid CorrelationId { get; }

        Guid? CausationId { get; }
    }
}
