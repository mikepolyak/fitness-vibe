using MediatR;
using FitnessVibe.Application.Common;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to start a new activity session - like pressing "start" on your fitness tracker.
    /// This is the moment when intention becomes action, when you transition from planning 
    /// to doing. Whether it's a morning run, gym session, or yoga practice, this command
    /// captures that powerful moment of beginning.
    /// </summary>
    public class StartActivityCommand : IRequest<StartActivityResponse>, IUserOwnedEntity
    {
        /// <summary>
        /// User ID of the activity owner.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Type of activity (Running, Cycling, Swimming, Gym, Yoga, etc.)
        /// </summary>
        public string ActivityType { get; set; } = string.Empty;

        /// <summary>
        /// Custom name for this session
        /// </summary>
        public string? ActivityName { get; set; }

        /// <summary>
        /// Starting latitude if GPS tracking is enabled
        /// </summary>
        public double? StartLatitude { get; set; }

        /// <summary>
        /// Starting longitude if GPS tracking is enabled
        /// </summary>
        public double? StartLongitude { get; set; }

        /// <summary>
        /// Starting altitude in meters if GPS tracking is enabled
        /// </summary>
        public double? StartAltitude { get; set; }

        /// <summary>
        /// When the session is planned to start
        /// </summary>
        public DateTime? PlannedStartTime { get; set; }

        /// <summary>
        /// Planned duration of the session
        /// </summary>
        public TimeSpan? PlannedDuration { get; set; }

        /// <summary>
        /// Optional notes about the session
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Additional metadata key-value pairs
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Whether friends can see this live session
        /// </summary>
        public bool IsPublic { get; set; } = true;

        /// <summary>
        /// Tags to categorize/identify the activity
        /// </summary>
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Response when an activity session begins - like getting confirmation that your tracker is recording.
    /// This contains all the essential information about your newly started fitness adventure.
    /// </summary>
    public class StartActivityResponse
    {
        /// <summary>
        /// ID of the new activity session
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Type of activity (e.g., Running, Cycling)
        /// </summary>
        public string ActivityType { get; set; } = string.Empty;

        /// <summary>
        /// Custom name for the session
        /// </summary>
        public string ActivityName { get; set; } = string.Empty;

        /// <summary>
        /// When the session started
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Current status (Active, Paused, etc.)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Whether GPS tracking is enabled
        /// </summary>
        public bool IsGpsEnabled { get; set; }

        /// <summary>
        /// Whether friends can send cheers
        /// </summary>
        public bool CanReceiveCheers { get; set; }

        /// <summary>
        /// Link to watch live session
        /// </summary>
        public string LiveSessionUrl { get; set; } = string.Empty;

        /// <summary>
        /// Expected workout statistics
        /// </summary>
        public EstimatedStatsResponse EstimatedStats { get; set; } = new();

        /// <summary>
        /// Personalized message to boost motivation
        /// </summary>
        public string MotivationalMessage { get; set; } = string.Empty;

        /// <summary>
        /// Suggested targets for this session
        /// </summary>
        public List<string> RecommendedTargets { get; set; } = new();
    }

    /// <summary>
    /// Estimated statistics for the planned workout session.
    /// Like having a personal trainer give you realistic expectations for your workout.
    /// </summary>
    public class EstimatedStatsResponse
    {
        public int EstimatedCaloriesPerHour { get; set; }
        public double EstimatedDistanceIfPlanned { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public int EstimatedXpReward { get; set; }
        public List<string> PossibleBadges { get; set; } = new();
    }
}