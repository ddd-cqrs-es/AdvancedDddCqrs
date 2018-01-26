using System;
using System.Collections.Generic;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class TopicDispatcher : ITopicDispatcher
    {
        private readonly IDictionary<string, Multiplexer<IMessage>> _subscriptions = new Dictionary<string, Multiplexer<IMessage>>();

        public void Publish<T>(T message) where T : class, IMessage
        {
            IEnumerable<string> topics = GetDefaultTopics(message);

            foreach (string topic in topics)
            {
                Multiplexer<IMessage> handlers;
                if (_subscriptions.TryGetValue(topic, out handlers))
                {
                    ////Console.WriteLine("Dispatching {0}:{1} to handlers for topic {2}", message, message.CorrelationId, topic);
                    handlers.Handle(message);
                }
            }
        }

        public void Subscribe<T>(IHandler<T> handler) where T : class, IMessage
        {
            Subscribe(TopicFromTypeName(typeof(T)), handler);
        }

        public void Subscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage
        {
            var imessageshandler = new NarrowingHandler<IMessage, T>(handler);

            Multiplexer<IMessage> existingHandler;
            if (_subscriptions.TryGetValue(topic, out existingHandler))
            {
                Multiplexer<IMessage> clone = existingHandler.Clone();
                clone.AddHandler(imessageshandler);
                _subscriptions[topic] = clone;
            }
            else
            {
                var handlers = new Multiplexer<IMessage>(new[] { imessageshandler });
                _subscriptions.Add(topic, handlers);
            }
        }

        public void Unsubscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage
        {
            Multiplexer<IMessage> multiplexer;
            if (_subscriptions.TryGetValue(topic, out multiplexer))
            {
                Multiplexer<IMessage> clone = multiplexer.Clone();
                clone.RemoveHandler(handler);
                _subscriptions[topic] = clone;
            }
        }

        private static string TopicFromTypeName(Type messageType)
        {
            return messageType.FullName;
        }

        private static string TopicFromCorrelationId<T>(T message) where T : class, IMessage
        {
            return message.CorrelationId.ToString();
        }

        private static IEnumerable<string> GetDefaultTopics<T>(T message) where T : class, IMessage
        {
            var topics = new List<string>
            {
                TopicFromTypeName(message.GetType()),
                TopicFromCorrelationId(message)
            };

            return topics;
        }

        //private static IEnumerable<string> GetDefaultTopics<T>(T message) where T : class, IMessage
        //{
        //    var types = new TypeInspector().GetTypeHierarchy(message.GetType());
        //    var topics = new List<string>();

        //    foreach (var type in types)
        //    {
        //        TopicFromTypeName(type);
        //    }

        //    topics.Add(TopicFromCorrelationId(message));
        //    return topics;
        //}
    }
}
