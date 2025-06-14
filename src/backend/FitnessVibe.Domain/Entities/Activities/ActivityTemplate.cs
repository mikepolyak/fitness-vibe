using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Enums;
using System.Collections.Generic;

namespace FitnessVibe.Domain.Entities.Activities
{
    /// <summary>
    /// Template for pre-defined activities that users can start from.
    /// These serve as blueprints for common workouts and exercises.
    /// </summary>
    public class ActivityTemplate : BaseEntity
    {
        /// <summary>
        /// The name of the activity template
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Detailed description of what this activity template involves
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// The type of activity this template creates
        /// </summary>
        public ActivityType Type { get; private set; }

        /// <summary>
        /// The category of activity this template creates
        /// </summary>
        public ActivityCategory Category { get; private set; }

        /// <summary>
        /// URL to the template's icon or preview image
        /// </summary>
        public string? IconUrl { get; private set; }

        /// <summary>
        /// Estimated duration in minutes
        /// </summary>
        public int EstimatedDurationMinutes { get; private set; }

        /// <summary>
        /// Estimated calories burned
        /// </summary>
        public int EstimatedCaloriesBurned { get; private set; }

        /// <summary>
        /// Difficulty level from 1 (easiest) to 5 (hardest)
        /// </summary>
        public int DifficultyLevel { get; private set; }

        /// <summary>
        /// Equipment needed for this activity
        /// </summary>
        private readonly List<string> _requiredEquipment = new();
        public IReadOnlyList<string> RequiredEquipment => _requiredEquipment.AsReadOnly();

        /// <summary>
        /// Tags for searching and categorizing templates
        /// </summary>
        private readonly List<string> _tags = new();
        public IReadOnlyList<string> Tags => _tags.AsReadOnly();

        /// <summary>
        /// Whether this template is featured and should be highlighted to users
        /// </summary>
        public bool IsFeatured { get; private set; }

        /// <summary>
        /// Usage count - how many times this template has been used
        /// </summary>
        public int UsageCount { get; private set; }

        /// <summary>
        /// Average rating from users (1-5)
        /// </summary>
        public decimal AverageRating { get; private set; }

        /// <summary>
        /// Total number of ratings received
        /// </summary>
        public int RatingCount { get; private set; }

        /// <summary>
        /// Creates a new activity instance from this template
        /// </summary>
        public Activity CreateActivity(Guid userId)
        {
            return Activity.Create(
                name: Name,
                type: Type,
                category: Category,
                difficultyLevel: DifficultyLevel,
                userId: userId,
                description: Description,
                iconUrl: IconUrl,
                isFeatured: false); // User instances are never featured
        }

        /// <summary>
        /// Records usage of this template
        /// </summary>
        public void IncrementUsageCount()
        {
            UsageCount++;
        }

        /// <summary>
        /// Adds a rating for this template
        /// </summary>
        public void AddRating(int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

            var totalRating = (AverageRating * RatingCount) + rating;
            RatingCount++;
            AverageRating = totalRating / RatingCount;
        }

        /// <summary>
        /// Creates a new activity template
        /// </summary>
        public static ActivityTemplate Create(
            string name,
            string description,
            ActivityType type,
            ActivityCategory category,
            int estimatedDurationMinutes,
            int estimatedCaloriesBurned,
            int difficultyLevel,
            IEnumerable<string>? requiredEquipment = null,
            IEnumerable<string>? tags = null,
            string? iconUrl = null,
            bool isFeatured = false)
        {
            var template = new ActivityTemplate
            {
                Name = name,
                Description = description,
                Type = type,
                Category = category,
                EstimatedDurationMinutes = estimatedDurationMinutes,
                EstimatedCaloriesBurned = estimatedCaloriesBurned,
                DifficultyLevel = difficultyLevel,
                IconUrl = iconUrl,
                IsFeatured = isFeatured,
                UsageCount = 0,
                AverageRating = 0,
                RatingCount = 0
            };

            if (requiredEquipment != null)
                template._requiredEquipment.AddRange(requiredEquipment);

            if (tags != null)
                template._tags.AddRange(tags);

            return template;
        }
    }
}
