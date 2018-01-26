using System;
using System.Collections.Generic;
using System.Threading;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Cook : IHandler<CookFood>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly IDictionary<string, IList<string>> _recipes = new Dictionary<string, IList<string>>();
        private readonly int _sleepDuration;

        public Cook(ITopicDispatcher dispatcher, int sleepDuration)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _dispatcher = dispatcher;
            _sleepDuration = sleepDuration;

            _recipes.Add("Beans on Toast", new[] { "Beans", "Toast" });
        }

        public bool Handle(CookFood message)
        {
            Thread.Sleep(_sleepDuration);

            Order order = message.Order;

            foreach (OrderItem item in order.Items)
            {
                IList<string> ingredients;
                if (_recipes.TryGetValue(item.Name, out ingredients))
                {
                    item.IngredientsUsed = ingredients;
                }
                else
                {
                    throw new Exception(string.Format("Cook doesn't know how to make {0}", item.Name));
                }
            }

            Console.Write("+");
            _dispatcher.Publish(new Cooked(order, message.CorrelationId, message.MessageId));

            return true;
        }
    }
}
