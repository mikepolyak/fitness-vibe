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
        /// <summary>
        /// Gets the unique identifier for this registration event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the UTC timestamp when the user registered
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the user who registered
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Creates a new user registration event
        /// </summary>
        /// <param name="user">The user who registered</param>
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
        /// <summary>
        /// Gets the unique identifier for this level-up event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the UTC timestamp when the user leveled up
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the user who leveled up
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets the user's level before leveling up
        /// </summary>
        public int PreviousLevel { get; }

        /// <summary>
        /// Gets the user's new level after leveling up
        /// </summary>
        public int NewLevel { get; }

        /// <summary>
        /// Creates a new level-up event
        /// </summary>
        /// <param name="user">The user who leveled up</param>
        /// <param name="previousLevel">The user's level before leveling up</param>
        /// <param name="newLevel">The user's new level</param>
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
