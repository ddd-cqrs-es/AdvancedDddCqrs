using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class ReportingSystem : IHandler<Report>
    {
        private readonly ITopicDispatcher _dispatcher;

        public ReportingSystem(ITopicDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool Handle(Report message)
        {
            _dispatcher.Publish(new Completed(message.Order, message.CorrelationId, message.MessageId));
            return true;
        }
    }
}
