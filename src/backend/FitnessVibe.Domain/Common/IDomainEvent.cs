using System;

namespace FitnessVibe.Domain.Common
{
    /// <summary>
    /// Domain event interface - think of events as the nervous system of our app.
    /// Just like how your nervous system sends signals when something important happens,
    /// domain events notify other parts of the system when significant domain actions occur.
    /// </summary>
    public interface IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
    }
}
