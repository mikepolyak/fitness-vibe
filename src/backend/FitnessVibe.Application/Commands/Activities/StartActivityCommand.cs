using MediatR;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to start a new activity session - like pressing "start" on your fitness tracker.
    /// This is the moment when intention becomes action, when you transition from planning 
    /// to doing. Whether it's a morning run, gym session, or yoga practice, this command
    /// captures that powerful moment of beginning.
    /// </summary>
    public class StartActivityCommand : IRequest<StartActivityResponse>
    {
        public int UserId { get; set; }
        public string ActivityType { get; set; } = string.Empty; // Running, Cycling, Swimming, Gym, Yoga, etc.
        public string? ActivityName { get; set; } // Custom name for this session
        public double? StartLatitude { get; set; }
        public double? StartLongitude { get; set; }
        public double? StartAltitude { get; set; }
        public DateTime? PlannedStartTime { get; set; }
        public TimeSpan? PlannedDuration { get; set; }
        public string? Notes { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public bool IsPublic { get; set; } = true; // Whether friends can see this live session
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Response when an activity session begins - like getting confirmation that your tracker is recording.
    /// This contains all the essential information about your newly started fitness adventure.
    /// </summary>
    public class StartActivityResponse
    {
        public int SessionId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string ActivityName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Paused, etc.
        public bool IsGpsEnabled { get; set; }
        public bool CanReceiveCheers { get; set; }
        public string LiveSessionUrl { get; set; } = string.Empty;
        public EstimatedStatsResponse EstimatedStats { get; set; } = new();
        public string MotivationalMessage { get; set; } = string.Empty;
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