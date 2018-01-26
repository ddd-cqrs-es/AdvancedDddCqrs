using System;

namespace AdvancedDddCqrs.Messages
{
    public interface IHaveTTL
    {
        bool HasExpired();

        void SetExpiry(TimeSpan duration);
    }
}
