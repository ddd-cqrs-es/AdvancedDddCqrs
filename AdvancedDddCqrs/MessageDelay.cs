using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class MessageDelay : IHandler<IMessage>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly List<EchoWrapper> _messagesToEcho = new List<EchoWrapper>();

        public MessageDelay(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _dispatcher = dispatcher;

            Task.Factory.StartNew(() =>
            {
                while (!Cancelled)
                {
                    IEnumerable<EchoWrapper> messages = _messagesToEcho.Where(x => x.HasExpired());

                    foreach (EchoWrapper message in messages)
                    {
                        _dispatcher.Publish(((dynamic)message.Echo).Inner);
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public bool Cancelled { get; set; }

        public bool Handle(IMessage message)
        {
            var echoWrapper = new EchoWrapper(message);
            echoWrapper.SetExpiry(((dynamic)message).Delay);
            _messagesToEcho.Add(echoWrapper);

            return true;
        }
    }

    internal class EchoWrapper
    {
        public EchoWrapper(IMessage message)
        {
            throw new NotImplementedException();
        }

        public object Echo { get; private set; }

        public bool HasExpired()
        {
            throw new NotImplementedException();
        }

        public void SetExpiry(TimeSpan delay)
        {
            throw new NotImplementedException();
        }
    }

    public class Echo<T> : IMessage
    {
        public int RetryCount { get; set; }

        public int MaxRetries { get; set; }

        public TimeSpan Delay { get; private set; }

        public T Inner { get; private set; }

        public Guid MessageId { get; private set; }

        public Guid CorrelationId { get; private set; }

        public Guid? CausationId { get; private set; }
    }
}
