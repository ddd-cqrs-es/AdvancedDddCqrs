using System;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Logger : IHandler<LogMessage>
    {
        private readonly object _syncLock = new object();

        public bool Handle(LogMessage message)
        {
            lock (_syncLock)
            {
                ConsoleColor prevColor = Console.ForegroundColor;

                try
                {
                    Console.ForegroundColor = message.Color;
                }
                finally
                {
                    Console.ForegroundColor = prevColor;
                }

                return true;
            }
        }
    }
}
