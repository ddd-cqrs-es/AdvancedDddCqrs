using System;

using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public static class TTLSettingHandler
    {
        public static TTLSettingHandler<T> Wrap<T>(IHandler<T> handler, int durationSeconds)
        {
            return new TTLSettingHandler<T>(handler, durationSeconds);
        }
    }

    public class TTLSettingHandler<T> : IHandler<T>
    {
        private readonly int _durationSeconds;
        private readonly IHandler<T> _handler;

        public TTLSettingHandler(IHandler<T> handler, int durationSeconds)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            _handler = handler;

            _durationSeconds = durationSeconds;
        }

        public bool Handle(T message)
        {
            var hasTTL = message as IHaveTTL;
            if (hasTTL != null)
            {
                hasTTL.SetExpiry(TimeSpan.FromSeconds(_durationSeconds));
            }

            _handler.Handle(message);
            return true;
        }

        public override string ToString()
        {
            return string.Format("TTLSettingHandler({0})", _handler);
        }
    }
}
