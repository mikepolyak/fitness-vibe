using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.DTOs.Activities
{
    /// <summary>
    /// Represents a user's completed or in-progress activity
    /// </summary>
    public class UserActivityResponse
    {
        /// <summary>
        /// The unique identifier of the activity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The type of activity
        /// </summary>
        public ActivityType ActivityType { get; set; }

        /// <summary>
        /// Current status of the activity
        /// </summary>
        public ActivityStatus Status { get; set; }

        /// <summary>
        /// When the activity started
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// When the activity was completed (if applicable)
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Total duration including pauses
        /// </summary>
        public TimeSpan TotalDuration { get; set; }

        /// <summary>
        /// Active duration excluding pauses
        /// </summary>
        public TimeSpan ActiveDuration { get; set; }

        /// <summary>
        /// Total calories burned
        /// </summary>
        public double CaloriesBurned { get; set; }

        /// <summary>
        /// Distance covered in meters (if applicable)
        /// </summary>
        public double? DistanceCovered { get; set; }

        /// <summary>
        /// Average heart rate for the session
        /// </summary>
        public int? AverageHeartRate { get; set; }

        /// <summary>
        /// Maximum heart rate reached
        /// </summary>
        public int? MaxHeartRate { get; set; }

        /// <summary>
        /// Brief description or notes about the activity
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Rating given by the user (1-5)
        /// </summary>
        public int? Rating { get; set; }
    }
}
