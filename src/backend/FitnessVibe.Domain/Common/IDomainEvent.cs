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
        /// <summary>
        /// Gets the unique identifier for this event instance
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the UTC timestamp when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; }
    }
}
