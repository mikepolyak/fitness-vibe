using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Domain.Entities.Activities
{
    /// <summary>
    /// Represents a user's specific instance of an activity, tracking their progress and metrics.
    /// Think of this as the 'gameplay' record for each activity - when they did it, how they performed, etc.
    /// </summary>
    public class UserActivity : BaseEntity
    {
        /// <summary>
        /// The unique identifier of the User who performed this activity
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Reference to the User who performed this activity
        /// </summary>
        public virtual User User { get; private set; } = null!;

        /// <summary>
        /// The unique identifier of the Activity that was performed
        /// </summary>
        public Guid ActivityId { get; private set; }

        /// <summary>
        /// Reference to the Activity that was performed
        /// </summary>
        public virtual Activity Activity { get; private set; } = null!;

        /// <summary>
        /// When the user started this activity instance
        /// </summary>
        public DateTime StartedAt { get; private set; }

        /// <summary>
        /// When the user completed this activity instance (null if not completed)
        /// </summary>
        public DateTime? CompletedAt { get; private set; }

        /// <summary>
        /// Duration in minutes of the activity
        /// </summary>
        public int DurationMinutes { get; private set; }

        /// <summary>
        /// Intensity level of the activity (1-5)
        /// </summary>
        public int IntensityLevel { get; private set; }

        /// <summary>
        /// Number of calories burned during this activity
        /// </summary>
        public double CaloriesBurned { get; private set; }

        /// <summary>
        /// User's notes or comments about this activity session
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Experience points earned from this activity
        /// </summary>
        public int ExperiencePointsEarned { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        protected UserActivity() { }

        /// <summary>
        /// Creates a new user activity
        /// </summary>
        public static UserActivity Create(Guid userId, Guid activityId, DateTime startedAt, int durationMinutes, int intensityLevel)
        {
            var userActivity = new UserActivity
            {
                UserId = userId,
                ActivityId = activityId,
                StartedAt = startedAt,
                DurationMinutes = durationMinutes,
                IntensityLevel = intensityLevel
            };

            // CaloriesBurned will be calculated based on duration, intensity, and user's metrics
            // ExperiencePointsEarned will be calculated based on activity completion and performance
            return userActivity;
        }

        /// <summary>
        /// Completes the activity with the calculated metrics
        /// </summary>
        public void Complete(DateTime completedAt, double caloriesBurned, int experiencePoints)
        {
            if (completedAt < StartedAt)
                throw new ArgumentException("Completion time cannot be before start time", nameof(completedAt));

            CompletedAt = completedAt;
            CaloriesBurned = caloriesBurned;
            ExperiencePointsEarned = experiencePoints;
        }

        /// <summary>
        /// Updates the user's notes for this activity
        /// </summary>
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }
    }
}
