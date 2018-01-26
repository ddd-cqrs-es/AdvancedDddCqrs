using System;

namespace AdvancedDddCqrs.Messages
{
    public class LogMessage : IMessage
    {
        public LogMessage(IMessage initiatingMessage, string logMessage, ConsoleColor color)
        {
            MessageId = Guid.NewGuid();
            CorrelationId = initiatingMessage.CorrelationId;
            CausationId = initiatingMessage.MessageId;
            Message = logMessage;
            Color = color;
        }

        public string Message { get; set; }

        public ConsoleColor Color { get; set; }

        public Guid MessageId { get; }

        public Guid CorrelationId { get; }

        public Guid? CausationId { get; }
    }
}
