using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Activities;

namespace FitnessVibe.Domain.Events
{
    /// <summary>
    /// Fired when a user completes an activity - the heartbeat of our fitness app!
    /// This event triggers XP awards, badge checks, goal progress updates,
    /// social feed posts, and streak calculations.
    /// Think of it as the moment of celebration after a workout.
    /// </summary>
    public class ActivityCompletedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public UserActivity UserActivity { get; }

        public ActivityCompletedEvent(UserActivity userActivity)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            UserActivity = userActivity ?? throw new ArgumentNullException(nameof(userActivity));
        }
    }
}
