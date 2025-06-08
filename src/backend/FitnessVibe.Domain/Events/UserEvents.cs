using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Domain.Events
{
    /// <summary>
    /// Fired when a new user joins our fitness community.
    /// Think of this as the "welcome party" signal that triggers onboarding flows,
    /// welcome emails, initial badge awards, etc.
    /// </summary>
    public class UserRegisteredEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public User User { get; }

        public UserRegisteredEvent(User user)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }

    /// <summary>
    /// Fired when a user reaches a new level in their fitness journey.
    /// Like celebrating a birthday or promotion - this triggers rewards,
    /// notifications, and social sharing opportunities.
    /// </summary>
    public class UserLeveledUpEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public User User { get; }
        public int PreviousLevel { get; }
        public int NewLevel { get; }

        public UserLeveledUpEvent(User user, int previousLevel, int newLevel)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            User = user ?? throw new ArgumentNullException(nameof(user));
            PreviousLevel = previousLevel;
            NewLevel = newLevel;
        }
    }
}
