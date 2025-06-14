using MediatR;
using FitnessVibe.Application.DTOs.Activities;

namespace FitnessVibe.Application.Queries.Activities
{
    /// <summary>
    /// Query to get real-time information about an active workout session.
    /// Like having a digital personal trainer monitoring your live workout progress
    /// and providing real-time feedback and encouragement.
    /// </summary>
    public class GetLiveActivityQuery : IRequest<LiveActivityResponse>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public bool IncludeRealTimeMetrics { get; set; } = true;
        public bool IncludeFriendsWatching { get; set; } = true;
        public bool IncludeCheers { get; set; } = true;
    }
}
        public double? TargetDistance { get; set; }
        public int? TargetCalories { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Current real-time workout metrics.
    /// Like the live readout on your fitness tracker showing exactly what's happening now.
    /// </summary>
    public class LiveMetricsDto
    {
        public TimeSpan CurrentDuration { get; set; }
        public double CurrentDistance { get; set; }
        public string DistanceUnit { get; set; } = "km";
        public int CurrentCalories { get; set; }
        public double? CurrentSpeed { get; set; }
        public double? CurrentPace { get; set; }
        public string PaceUnit { get; set; } = "min/km";
        public int? CurrentHeartRate { get; set; }
        public double? CurrentAltitude { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public int? PerceivedExertion { get; set; } // 1-10 scale
        public string? CurrentMood { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// A cheer or encouragement message from friends.
    /// Like having your personal cheering squad motivating you during your workout!
    /// </summary>
    public class LiveCheerDto
    {
        public int Id { get; set; }
        public string FromUserName { get; set; } = string.Empty;
        public string FromUserAvatarUrl { get; set; } = string.Empty;
        public string CheerType { get; set; } = string.Empty; // Text, Audio, Emoji, PowerUp
        public string Message { get; set; } = string.Empty;
        public string? AudioUrl { get; set; }
        public string? EmojiCode { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public int PowerUpValue { get; set; } // XP bonus from this cheer
    }

    /// <summary>
    /// Friends who are currently watching/following your live workout.
    /// Like having spectators cheering you on from the sidelines!
    /// </summary>
    public class FriendWatchingDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime StartedWatchingAt { get; set; }
        public bool CanSendCheers { get; set; }
        public string? CurrentActivity { get; set; } // If they're also working out
    }

    /// <summary>
    /// Progress towards workout goals and targets.
    /// Like your progress bar showing how close you are to your goals.
    /// </summary>
    public class WorkoutProgressDto
    {
        public double DurationProgressPercentage { get; set; }
        public double DistanceProgressPercentage { get; set; }
        public double CaloriesProgressPercentage { get; set; }
        public List<GoalProgressDto> GoalProgress { get; set; } = new();
        public EstimatedCompletionDto EstimatedCompletion { get; set; } = new();
        public string OverallPace { get; set; } = string.Empty; // On Track, Ahead, Behind
    }

    /// <summary>
    /// Progress towards a specific goal.
    /// </summary>
    public class GoalProgressDto
    {
        public string GoalType { get; set; } = string.Empty;
        public string GoalName { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
        public double TargetValue { get; set; }
        public double ProgressPercentage { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsAchieved { get; set; }
        public TimeSpan? EstimatedTimeToComplete { get; set; }
    }

    /// <summary>
    /// Estimated completion information.
    /// </summary>
    public class EstimatedCompletionDto
    {
        public DateTime? EstimatedFinishTime { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public double? DistanceRemaining { get; set; }
        public int? CaloriesRemaining { get; set; }
        public double Confidence { get; set; } // 0-100% confidence in estimate
    }

    /// <summary>
    /// Upcoming milestones during the workout.
    /// Like checkpoints along your fitness journey to celebrate!
    /// </summary>
    public class MilestoneDto
    {
        public string MilestoneType { get; set; } = string.Empty; // Distance, Time, Calories, Heart Rate Zone
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public double CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
        public TimeSpan? EstimatedTimeToReach { get; set; }
        public int XpReward { get; set; }
        public bool IsPersonalRecord { get; set; }
    }

    /// <summary>
    /// Safety and wellness monitoring information.
    /// Like having a fitness supervisor ensuring you're working out safely.
    /// </summary>
    public class SafetyStatusDto
    {
        public string OverallStatus { get; set; } = string.Empty; // Good, Caution, Warning
        public List<SafetyAlertDto> ActiveAlerts { get; set; } = new();
        public HeartRateZoneDto? HeartRateZone { get; set; }
        public DateTime? LastCheckIn { get; set; }
        public bool AutoPauseEnabled { get; set; }
        public bool EmergencyContactsNotified { get; set; }
    }

    /// <summary>
    /// Safety alert information.
    /// </summary>
    public class SafetyAlertDto
    {
        public string AlertType { get; set; } = string.Empty; // HeartRate, Pace, Duration, Weather
        public string Severity { get; set; } = string.Empty; // Info, Warning, Critical
        public string Message { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
    }

    /// <summary>
    /// Heart rate zone information.
    /// </summary>
    public class HeartRateZoneDto
    {
        public string CurrentZone { get; set; } = string.Empty; // Rest, Fat Burn, Cardio, Peak
        public int CurrentHeartRate { get; set; }
        public int ZoneMinHeartRate { get; set; }
        public int ZoneMaxHeartRate { get; set; }
        public double TimeInZonePercentage { get; set; }
        public string ZoneColor { get; set; } = string.Empty;
        public bool IsTargetZone { get; set; }
    }
}
