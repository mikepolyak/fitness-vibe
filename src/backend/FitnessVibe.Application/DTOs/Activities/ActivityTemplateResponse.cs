using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.DTOs.Activities
{
    /// <summary>
    /// Represents a reusable activity template
    /// </summary>
    public class ActivityTemplateResponse
    {
        /// <summary>
        /// The unique identifier of the template
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the template
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Detailed description of the workout
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of activity this template is for
        /// </summary>
        public ActivityType ActivityType { get; set; }

        /// <summary>
        /// Difficulty level of the workout
        /// </summary>
        public string DifficultyLevel { get; set; }

        /// <summary>
        /// Estimated duration in minutes
        /// </summary>
        public int EstimatedDuration { get; set; }

        /// <summary>
        /// Estimated calories that can be burned
        /// </summary>
        public int EstimatedCalories { get; set; }

        /// <summary>
        /// Equipment needed for this workout
        /// </summary>
        public List<string> RequiredEquipment { get; set; }

        /// <summary>
        /// User ID who created the template
        /// </summary>
        public Guid CreatedById { get; set; }

        /// <summary>
        /// When the template was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Rating of the template (average of user ratings)
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Number of times this template has been used
        /// </summary>
        public int UsageCount { get; set; }
    }
}
