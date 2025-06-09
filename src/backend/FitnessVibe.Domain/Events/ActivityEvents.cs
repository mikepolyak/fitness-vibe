using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Domain.Events
{
    /// <summary>
    /// Contains all domain events related to activities
    /// </summary>
    public static class ActivityEvents
    {
        /// <summary>
        /// Fired when a new activity type is created
        /// </summary>
        public class ActivityCreated : IDomainEvent
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
            /// Gets the ID of the activity that was created
            /// </summary>
            public Guid ActivityId { get; }

            /// <summary>
            /// Gets the name of the created activity
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the type of the created activity
            /// </summary>
            public ActivityType Type { get; }

            /// <summary>
            /// Gets the category of the created activity
            /// </summary>
            public ActivityCategory Category { get; }

            /// <summary>
            /// Creates a new ActivityCreated event
            /// </summary>
            /// <param name="activityId">The ID of the activity that was created</param>
            /// <param name="name">The name of the created activity</param>
            /// <param name="type">The type of the created activity</param>
            /// <param name="category">The category of the created activity</param>
            public ActivityCreated(Guid activityId, string name, ActivityType type, ActivityCategory category)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                ActivityId = activityId;
                Name = name;
                Type = type;
                Category = category;
            }
        }

        /// <summary>
        /// Fired when an activity's details are updated
        /// </summary>
        public class ActivityUpdated : IDomainEvent
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
            /// Gets the ID of the activity that was updated
            /// </summary>
            public Guid ActivityId { get; }

            /// <summary>
            /// Gets the new name of the activity
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the new type of the activity
            /// </summary>
            public ActivityType Type { get; }

            /// <summary>
            /// Gets the new category of the activity
            /// </summary>
            public ActivityCategory Category { get; }

            /// <summary>
            /// Creates a new ActivityUpdated event
            /// </summary>
            /// <param name="activityId">The ID of the activity that was updated</param>
            /// <param name="name">The new name of the activity</param>
            /// <param name="type">The new type of the activity</param>
            /// <param name="category">The new category of the activity</param>
            public ActivityUpdated(Guid activityId, string name, ActivityType type, ActivityCategory category)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                ActivityId = activityId;
                Name = name;
                Type = type;
                Category = category;
            }
        }

        /// <summary>
        /// Fired when an activity's featured status changes
        /// </summary>
        public class ActivityFeaturedStatusChanged : IDomainEvent
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
            /// Gets the ID of the activity whose featured status changed
            /// </summary>
            public Guid ActivityId { get; }

            /// <summary>
            /// Gets whether the activity is now featured
            /// </summary>
            public bool IsFeatured { get; }

            /// <summary>
            /// Creates a new ActivityFeaturedStatusChanged event
            /// </summary>
            /// <param name="activityId">The ID of the activity whose featured status changed</param>
            /// <param name="isFeatured">Whether the activity is now featured</param>
            public ActivityFeaturedStatusChanged(Guid activityId, bool isFeatured)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                ActivityId = activityId;
                IsFeatured = isFeatured;
            }
        }

        /// <summary>
        /// Fired when a user completes an activity - the heartbeat of our fitness app!
        /// This event triggers XP awards, badge checks, goal progress updates,
        /// social feed posts, and streak calculations.
        /// Think of it as the moment of celebration after a workout.
        /// </summary>
        public class ActivityCompleted : IDomainEvent
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
            /// Gets the completed user activity
            /// </summary>
            public UserActivity UserActivity { get; }

            /// <summary>
            /// Creates a new ActivityCompleted event
            /// </summary>
            /// <param name="userActivity">The user activity that was completed</param>
            public ActivityCompleted(UserActivity userActivity)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                UserActivity = userActivity ?? throw new ArgumentNullException(nameof(userActivity));
            }
        }

    /// <summary>
    /// Event raised when a new activity is created
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who created the activity</param>
    public record ActivityCreatedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is started
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who started the activity</param>
    public record ActivityStartedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is paused
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who paused the activity</param>
    public record ActivityPausedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when a paused activity is resumed
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who resumed the activity</param>
    public record ActivityResumedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is completed
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who completed the activity</param>
    public record ActivityCompletedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is cancelled
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who cancelled the activity</param>
    /// <param name="Reason">Reason for cancellation</param>
    public record ActivityCancelledEvent(Guid ActivityId, Guid UserId, string Reason) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    }
}
