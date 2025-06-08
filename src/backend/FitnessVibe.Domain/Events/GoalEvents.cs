using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Domain.Events
{
    /// <summary>
    /// Fired when a user completes a goal - like crossing the finish line!
    /// This triggers celebrations, badge awards, social sharing, and sets up new challenges.
    /// </summary>
    public class GoalCompletedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the UTC timestamp when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the goal that was completed
        /// </summary>
        public UserGoal Goal { get; }

        /// <summary>
        /// Gets the ID of the user who completed the goal
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Gets the title of the completed goal
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the type of the completed goal
        /// </summary>
        public GoalType Type { get; }

        /// <summary>
        /// Gets the final achieved value when the goal was completed
        /// </summary>
        public decimal FinalValue { get; }

        /// <summary>
        /// Creates a new instance of the GoalCompletedEvent
        /// </summary>
        /// <param name="goal">The goal that was completed</param>
        public GoalCompletedEvent(UserGoal goal)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
            UserId = goal.UserId;
            Title = goal.Title;
            Type = goal.Type;
            FinalValue = goal.CurrentValue;
        }
    }

    /// <summary>
    /// Fired when a user abandons a goal.
    /// This can trigger encouragement flows, goal adjustment suggestions, or data for analytics.
    /// </summary>
    public class GoalAbandonedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the UTC timestamp when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the goal that was abandoned
        /// </summary>
        public UserGoal Goal { get; }

        /// <summary>
        /// Gets the ID of the user who abandoned the goal
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Gets the title of the abandoned goal
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the type of the abandoned goal
        /// </summary>
        public GoalType Type { get; }

        /// <summary>
        /// Gets the progress made before abandoning the goal
        /// </summary>
        public decimal Progress { get; }

        /// <summary>
        /// Creates a new instance of the GoalAbandonedEvent
        /// </summary>
        /// <param name="goal">The goal that was abandoned</param>
        public GoalAbandonedEvent(UserGoal goal)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
            UserId = goal.UserId;
            Title = goal.Title;
            Type = goal.Type;
            Progress = goal.CurrentValue;
        }
    }

    /// <summary>
    /// Fired when a user fails to complete a goal by its deadline.
    /// This can trigger motivation messages, goal adjustment suggestions, or analytics for understanding failure patterns.
    /// </summary>
    public class GoalFailedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the UTC timestamp when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the goal that was failed
        /// </summary>
        public UserGoal Goal { get; }

        /// <summary>
        /// Gets the ID of the user who failed the goal
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Gets the title of the failed goal
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the type of the failed goal
        /// </summary>
        public GoalType Type { get; }

        /// <summary>
        /// Gets the final progress achieved before failing
        /// </summary>
        public decimal FinalProgress { get; }

        /// <summary>
        /// Gets how much more was needed to reach the target
        /// </summary>
        public decimal RemainingToTarget { get; }

        /// <summary>
        /// Creates a new instance of the GoalFailedEvent
        /// </summary>
        /// <param name="goal">The goal that was failed</param>
        public GoalFailedEvent(UserGoal goal)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
            UserId = goal.UserId;
            Title = goal.Title;
            Type = goal.Type;
            FinalProgress = goal.CurrentValue;
            RemainingToTarget = goal.TargetValue - goal.CurrentValue;
        }
    }
}
