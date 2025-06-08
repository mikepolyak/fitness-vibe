using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Domain.Events
{
    /// <summary>
    /// Fired when a user completes a goal - like crossing the finish line!
    /// This triggers celebrations, badge awards, social sharing, and sets up new challenges.
    /// </summary>
    public class GoalCompletedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public UserGoal Goal { get; }

        public GoalCompletedEvent(UserGoal goal)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
        }
    }

    /// <summary>
    /// Fired when a user abandons a goal.
    /// This can trigger encouragement flows, goal adjustment suggestions, or data for analytics.
    /// </summary>
    public class GoalAbandonedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public UserGoal Goal { get; }

        public GoalAbandonedEvent(UserGoal goal)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
        }
    }
}
