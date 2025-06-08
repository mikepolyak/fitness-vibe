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
        /// Navigation property for all user activities of this type
        /// </summary>
        public virtual ICollection<UserActivity> UserActivities { get; private set; } = new List<UserActivity>();

        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected Activity() { }

        /// <summary>
        /// Creates a new activity template
        /// </summary>
        public static Activity Create(
            string name,
            ActivityType type,
            ActivityCategory category,
            int difficultyLevel,
            int estimatedCaloriesPerHour,
            string? description = null,
            string? iconUrl = null,
            bool isFeatured = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Activity name cannot be empty", nameof(name));

            if (difficultyLevel < 1 || difficultyLevel > 5)
                throw new ArgumentException("Difficulty level must be between 1 and 5", nameof(difficultyLevel));

            if (estimatedCaloriesPerHour <= 0)
                throw new ArgumentException("Estimated calories must be greater than 0", nameof(estimatedCaloriesPerHour));

            var activity = new Activity
            {
                Name = name,
                Type = type,
                Category = category,
                Description = description,
                IconUrl = iconUrl,
                IsFeatured = isFeatured,
                DifficultyLevel = difficultyLevel,
                EstimatedCaloriesPerHour = estimatedCaloriesPerHour
            };

            activity.AddDomainEvent(new ActivityEvents.ActivityCreated(activity.Id, name, type, category));

            return activity;
        }

        /// <summary>
        /// Updates the activity's details
        /// </summary>
        public void Update(
            string name,
            ActivityType type,
            ActivityCategory category,
            int difficultyLevel,
            int estimatedCaloriesPerHour,
            string? description = null,
            string? iconUrl = null,
            bool? isFeatured = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            Type = type;
            Category = category;

            if (difficultyLevel >= 1 && difficultyLevel <= 5)
                DifficultyLevel = difficultyLevel;

            if (estimatedCaloriesPerHour > 0)
                EstimatedCaloriesPerHour = estimatedCaloriesPerHour;

            Description = description;
            IconUrl = iconUrl;

            if (isFeatured.HasValue)
                IsFeatured = isFeatured.Value;

            AddDomainEvent(new ActivityEvents.ActivityUpdated(Id, name, type, category));
        }

        /// <summary>
        /// Set or unset the featured status of the activity
        /// </summary>
        public void SetFeatured(bool isFeatured)
        {
            if (IsFeatured != isFeatured)
            {
                IsFeatured = isFeatured;
                AddDomainEvent(new ActivityEvents.ActivityFeaturedStatusChanged(Id, isFeatured));
            }
        }
    }
}
