using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.DTOs.Activities
{
    /// <summary>
    /// Represents the current state of an active workout session
    /// </summary>
    public class LiveActivityResponse
    {
        /// <summary>
        /// The unique identifier of the activity session
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// The type of activity being performed
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
        /// Total duration of the activity including pauses
        /// </summary>
        public TimeSpan TotalDuration { get; set; }

        /// <summary>
        /// Active duration excluding pauses
        /// </summary>
        public TimeSpan ActiveDuration { get; set; }

        /// <summary>
        /// Last time the activity metrics were updated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Calories burned so far
        /// </summary>
        public double CaloriesBurned { get; set; }

        /// <summary>
        /// Distance covered in meters (if applicable)
        /// </summary>
        public double? DistanceCovered { get; set; }

        /// <summary>
        /// Current heart rate (if available)
        /// </summary>
        public int? CurrentHeartRate { get; set; }

        /// <summary>
        /// Average heart rate for the session
        /// </summary>
        public int? AverageHeartRate { get; set; }

        /// <summary>
        /// Current activity intensity level
        /// </summary>
        public string IntensityLevel { get; set; }
    }
}
