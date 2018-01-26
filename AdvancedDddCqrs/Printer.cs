using System;

using AdvancedDddCqrs.Messages;

using Newtonsoft.Json;

namespace AdvancedDddCqrs
{
    internal class Printer : IHandler<IMessage>
    {
        public bool Handle(IMessage message)
        {
            Console.WriteLine("{0}: {1}", message, JsonConvert.SerializeObject(message, Formatting.Indented));
            return true;
        }
    }
}
