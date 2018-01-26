using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public interface ITopicDispatcher
    {
        void Publish<T>(T message) where T : class, IMessage;

        void Subscribe<T>(IHandler<T> handler) where T : class, IMessage;

        void Subscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage;

        void Unsubscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage;
    }
}
