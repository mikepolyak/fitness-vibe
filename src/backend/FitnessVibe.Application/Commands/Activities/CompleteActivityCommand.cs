using MediatR;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to complete an active workout session - like crossing the finish line.
    /// This is the moment of triumph when you transition from "working out" to "worked out!"
    /// It's time to celebrate your effort, log your achievements, and earn your rewards.
    /// </summary>
    public class CompleteActivityCommand : IRequest<CompleteActivityResponse>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public DateTime? EndTime { get; set; } // Optional - defaults to now
        public double? EndLatitude { get; set; }
        public double? EndLongitude { get; set; }
        public double? EndAltitude { get; set; }
        public string? Notes { get; set; } // Post-workout reflection
        public int? ManualCalories { get; set; } // Override calculated calories
        public double? ManualDistance { get; set; } // Override calculated distance
        public int? PerceivedExertion { get; set; } // 1-10 scale, how hard it felt
        public string? MoodAfter { get; set; } // How they feel post-workout
        public List<string> PhotoUrls { get; set; } = new(); // Workout selfies!
        public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
        public bool ShareToFeed { get; set; } = true;
        public List<string> TagFriends { get; set; } = new(); // Tag workout buddies
    }

    /// <summary>
    /// Response after completing a workout - like getting your results and victory lap!
    /// This contains all the stats, rewards, and celebrations for your hard work.
    /// </summary>
    public class CompleteActivityResponse
    {
        public Guid ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public WorkoutStatsResponse Stats { get; set; } = new();
        public RewardsEarnedResponse Rewards { get; set; } = new();
        public PersonalRecordsResponse PersonalRecords { get; set; } = new();
        public string CelebrationMessage { get; set; } = string.Empty;
        public List<string> Achievements { get; set; } = new();
        public SocialShareResponse SocialShare { get; set; } = new();
        public NextWorkoutSuggestionResponse NextSuggestion { get; set; } = new();
        public bool CreatedNewPersonalRecord { get; set; }
        public int FriendsWhoWillSeeThis { get; set; }
    }

    /// <summary>
    /// Complete statistics from the completed workout session.
    /// Like getting a detailed report card from your personal trainer.
    /// </summary>
    public class WorkoutStatsResponse
    {
        public TimeSpan Duration { get; set; }
        public TimeSpan ActiveTime { get; set; }
        public TimeSpan RestTime { get; set; }
        public double Distance { get; set; }
        public string DistanceUnit { get; set; } = string.Empty;
        public int CaloriesBurned { get; set; }
        public double AverageSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public double ElevationGain { get; set; }
        public int? AverageHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
        public List<WorkoutSplitResponse> Splits { get; set; } = new();
        public string PerformanceRating { get; set; } = string.Empty; // Excellent, Good, etc.
    }

    /// <summary>
    /// Split times during the workout - like lap times for runners.
    /// </summary>
    public class WorkoutSplitResponse
    {
        public int SplitNumber { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public double Pace { get; set; }
        public string PaceUnit { get; set; } = string.Empty;
    }

    /// <summary>
    /// All the rewards earned from this workout session.
    /// Like collecting your prizes after winning a game!
    /// </summary>
    public class RewardsEarnedResponse
    {
        public int XpEarned { get; set; }
        public int XpBonus { get; set; }
        public string XpBonusReason { get; set; } = string.Empty;
        public List<BadgeEarnedResponse> BadgesEarned { get; set; } = new();
        public bool LeveledUp { get; set; }
        public int? NewLevel { get; set; }
        public string? NewLevelTitle { get; set; }
        public List<string> UnlockedFeatures { get; set; } = new();
        public int StreakDays { get; set; }
        public bool StreakMilestone { get; set; }
    }

    /// <summary>
    /// Information about a badge earned during this workout.
    /// </summary>
    public class BadgeEarnedResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int Points { get; set; }
        public bool IsFirstTime { get; set; }
    }

    /// <summary>
    /// Personal records achieved during this workout.
    /// Like updating your trophy case with new best performances!
    /// </summary>
    public class PersonalRecordsResponse
    {
        public List<PersonalRecordResponse> NewRecords { get; set; } = new();
        public List<PersonalRecordResponse> ImprovedRecords { get; set; } = new();
    }

    /// <summary>
    /// Details about a personal record.
    /// </summary>
    public class PersonalRecordResponse
    {
        public string RecordType { get; set; } = string.Empty; // Longest Distance, Fastest 5K, etc.
        public string NewValue { get; set; } = string.Empty;
        public string? PreviousValue { get; set; }
        public string Improvement { get; set; } = string.Empty;
        public DateTime AchievedAt { get; set; }
    }

    /// <summary>
    /// Information about sharing this workout on social media.
    /// </summary>
    public class SocialShareResponse
    {
        public bool WasShared { get; set; }
        public int PostId { get; set; }
        public string ShareableMessage { get; set; } = string.Empty;
        public string ShareableImageUrl { get; set; } = string.Empty;
        public List<string> SuggestedHashtags { get; set; } = new();
    }

    /// <summary>
    /// Suggestion for the next workout based on this performance.
    /// Like having your trainer plan your next session!
    /// </summary>
    public class NextWorkoutSuggestionResponse
    {
        public string SuggestedActivity { get; set; } = string.Empty;
        public string Reasoning { get; set; } = string.Empty;
        public DateTime SuggestedTime { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public TimeSpan EstimatedDuration { get; set; }
        public string FocusArea { get; set; } = string.Empty; // Recovery, Intensity, Endurance, etc.
    }
}