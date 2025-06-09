using System;
using System.Collections.Generic;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Enums;
using FitnessVibe.Domain.Events;

namespace FitnessVibe.Domain.Entities.Activities
{
    /// <summary>
    /// Activity entity - the core of our fitness tracking.
    /// Think of each activity as a chapter in the user's fitness story.
    /// Activities are where users spend their time, burn calories, and make progress.
    /// </summary>
    public class Activity : BaseEntity
    {
        /// <summary>
        /// The name of the activity (e.g., "Running", "Yoga", "Weight Training")
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Optional detailed description of the activity
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// The type of activity (Indoor, Outdoor, Virtual, Manual)
        /// </summary>
        public ActivityType Type { get; private set; }

        /// <summary>
        /// The category of the activity (Cardio, Strength, Flexibility, etc.)
        /// </summary>
        public ActivityCategory Category { get; private set; }

        /// <summary>
        /// URL to the activity's icon image
        /// </summary>
        public string? IconUrl { get; private set; }

        /// <summary>
        /// Whether this is a featured activity that should be highlighted to users
        /// </summary>
        public bool IsFeatured { get; private set; }

        /// <summary>
        /// The difficulty level of this activity (1-5)
        /// </summary>
        public int DifficultyLevel { get; private set; }

        /// <summary>
        /// The estimated calories burned per hour for a person of average fitness
        /// </summary>
        public int EstimatedCaloriesPerHour { get; private set; }

        /// <summary>
        /// The current status of the activity (Created, Active, Paused, Completed, Cancelled)
        /// </summary>
        public ActivityStatus Status { get; private set; }

        /// <summary>
        /// The user who owns this activity
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// When the activity was started
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// When the activity was completed or cancelled
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// List of pause periods during this activity
        /// </summary>
        private readonly List<(DateTime Start, DateTime? End)> _pausePeriods = new();

        /// <summary>
        /// Total duration excluding pauses, in minutes
        /// </summary>
        public int GetDurationInMinutes()
        {
            if (StartTime == null) return 0;

            var endPoint = EndTime ?? DateTime.UtcNow;
            var totalMinutes = (int)(endPoint - StartTime.Value).TotalMinutes;

            // Subtract pause durations
            foreach (var pause in _pausePeriods)
            {
                var pauseEnd = pause.End ?? endPoint;
                totalMinutes -= (int)(pauseEnd - pause.Start).TotalMinutes;
            }

            return totalMinutes;
        }

        /// <summary>
        /// Gets the total duration of all pauses in minutes
        /// </summary>
        /// <returns>Total pause duration in minutes</returns>
        public int GetPauseDurationInMinutes()
        {
            var totalPauseMinutes = 0;
            var now = DateTime.UtcNow;

            foreach (var pause in _pausePeriods)
            {
                var pauseEnd = pause.End ?? now;
                totalPauseMinutes += (int)(pauseEnd - pause.Start).TotalMinutes;
            }

            return totalPauseMinutes;
        }

        /// <summary>
        /// Gets the current progress percentage of the activity, from 0 to 1
        /// </summary>
        /// <returns>Progress as a decimal between 0 and 1</returns>
        public decimal GetProgressPercentage()
        {
            if (StartTime == null || Status == ActivityStatus.Created) return 0;
            if (Status == ActivityStatus.Completed) return 1;

            var totalDuration = GetDurationInMinutes();
            var targetDuration = 60; // Default 1 hour, could be configurable
            return Math.Min(1.0m, totalDuration / (decimal)targetDuration);
        }

        /// <summary>
        /// Calculates the total calories burned during the activity
        /// </summary>
        /// <returns>Estimated calories burned</returns>
        public decimal GetTotalCaloriesBurned()
        {
            var duration = GetDurationInMinutes();            var baseRate = Category switch
            {
                ActivityCategory.Cardio => 7.5m, // calories per minute
                ActivityCategory.Strength => 8.0m,
                ActivityCategory.CrossTraining => 12.0m,
                ActivityCategory.Sport => 9.0m,
                _ => 7.0m // default rate
            };

            return duration * baseRate;
        }

        /// <summary>
        /// Gets a list of achievements earned during this activity
        /// </summary>
        /// <returns>List of achievement descriptions</returns>
        public IEnumerable<string> GetAchievements()
        {
            var achievements = new List<string>();

            var duration = GetDurationInMinutes();
            if (duration >= 60) achievements.Add("1 Hour Warrior");
            if (duration >= 30) achievements.Add("30-Minute Champion");

            var calories = GetTotalCaloriesBurned();
            if (calories >= 500) achievements.Add("500 Calorie Burner");
            if (calories >= 300) achievements.Add("300 Calorie Club");

            return achievements;
        }

        /// <summary>
        /// Starts the activity
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if activity cannot be started in its current state</exception>
        public void Start()
        {
            if (Status != ActivityStatus.Created)
                throw new InvalidOperationException($"Cannot start activity - current status is {Status}");

            Status = ActivityStatus.Active;
            StartTime = DateTime.UtcNow;
            AddDomainEvent(new ActivityStartedEvent(Id, UserId));
        }

        /// <summary>
        /// Pauses the activity
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if activity cannot be paused in its current state</exception>
        public void Pause()
        {
            if (Status != ActivityStatus.Active)
                throw new InvalidOperationException($"Cannot pause activity - current status is {Status}");

            Status = ActivityStatus.Paused;
            _pausePeriods.Add((DateTime.UtcNow, null));
            AddDomainEvent(new ActivityPausedEvent(Id, UserId));
        }

        /// <summary>
        /// Resumes a paused activity
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if activity cannot be resumed in its current state</exception>
        public void Resume()
        {
            if (Status != ActivityStatus.Paused)
                throw new InvalidOperationException($"Cannot resume activity - current status is {Status}");

            Status = ActivityStatus.Active;
            if (_pausePeriods.Count > 0)
            {
                var lastPause = _pausePeriods[^1];
                _pausePeriods[^1] = (lastPause.Start, DateTime.UtcNow);
            }
            AddDomainEvent(new ActivityResumedEvent(Id, UserId));
        }

        /// <summary>
        /// Marks the activity as completed
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if activity cannot be completed in its current state</exception>
        public void Complete()
        {
            if (Status != ActivityStatus.Active)
                throw new InvalidOperationException($"Cannot complete activity - current status is {Status}");

            Status = ActivityStatus.Completed;
            EndTime = DateTime.UtcNow;
            AddDomainEvent(new ActivityCompletedEvent(Id, UserId));
        }

        /// <summary>
        /// Cancels the activity
        /// </summary>
        /// <param name="reason">Reason for cancellation</param>
        /// <exception cref="InvalidOperationException">Thrown if activity cannot be cancelled in its current state</exception>
        public void Cancel(string reason)
        {
            if (Status == ActivityStatus.Completed || Status == ActivityStatus.Cancelled)
                throw new InvalidOperationException($"Cannot cancel activity - current status is {Status}");

            Status = ActivityStatus.Cancelled;
            EndTime = DateTime.UtcNow;
            AddDomainEvent(new ActivityCancelledEvent(Id, UserId, reason));
        }

        /// <summary>
        /// Creates a new activity
        /// </summary>
        /// <param name="name">Name of the activity</param>
        /// <param name="type">Type of activity</param>
        /// <param name="category">Activity category</param>
        /// <param name="difficultyLevel">Difficulty level (1-5)</param>
        /// <param name="userId">Owner's user ID</param>
        /// <param name="description">Optional activity description</param>
        /// <param name="iconUrl">Optional URL to activity icon</param>
        /// <param name="isFeatured">Whether this is a featured activity</param>
        /// <returns>A new activity instance</returns>
        public static Activity Create(
            string name,
            ActivityType type,
            ActivityCategory category,
            int difficultyLevel,
            Guid userId,
            string? description = null,
            string? iconUrl = null,
            bool isFeatured = false)
        {
            var activity = new Activity
            {
                Name = name,
                Type = type,
                Category = category,
                DifficultyLevel = difficultyLevel,
                Description = description,
                IconUrl = iconUrl,
                IsFeatured = isFeatured,
                UserId = userId,
                Status = ActivityStatus.Created
            };

            activity.AddDomainEvent(new ActivityCreatedEvent(activity.Id, userId));
            return activity;
        }
    }

    internal class ActivityStartedEvent : IDomainEvent
    {
    }

}
