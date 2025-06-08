using MediatR;

namespace FitnessVibe.Application.Queries.Gamification
{
    /// <summary>
    /// Query to get leaderboards for competitive fitness tracking.
    /// Like checking the scoreboard at your gym to see how you rank against other members!
    /// </summary>
    public class GetLeaderboardQuery : IRequest<LeaderboardResponse>
    {
        public int UserId { get; set; }
        public string LeaderboardType { get; set; } = "Overall"; // Overall, Weekly, Monthly, Friends, Local
        public string MetricType { get; set; } = "XP"; // XP, Activities, Distance, Calories, Streaks
        public string? ActivityType { get; set; } // Filter by specific activity
        public string TimeFrame { get; set; } = "AllTime"; // AllTime, ThisWeek, ThisMonth, Today
        public string? Location { get; set; } // City, Region for local leaderboards
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public bool IncludeUserPosition { get; set; } = true;
        public bool IncludeNearbyRanks { get; set; } = true; // Show users around your rank
    }

    /// <summary>
    /// Leaderboard data with rankings and competitive information.
    /// Like a comprehensive scoreboard showing where everyone stands in the fitness competition!
    /// </summary>
    public class LeaderboardResponse
    {
        public LeaderboardMetadataDto Metadata { get; set; } = new();
        public List<LeaderboardEntryDto> Entries { get; set; } = new();
        public UserLeaderboardPositionDto UserPosition { get; set; } = new();
        public List<LeaderboardEntryDto> NearbyEntries { get; set; } = new(); // Users around you
        public LeaderboardStatsDto Statistics { get; set; } = new();
        public List<LeaderboardTierDto> Tiers { get; set; } = new();
        public string MotivationalMessage { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Metadata about the leaderboard.
    /// </summary>
    public class LeaderboardMetadataDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MetricType { get; set; } = string.Empty;
        public string MetricLabel { get; set; } = string.Empty;
        public string TimeFrame { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalParticipants { get; set; }
        public bool IsLive { get; set; } // Updates in real-time
        public string ResetFrequency { get; set; } = string.Empty; // Never, Daily, Weekly, Monthly
        public string NextResetDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// Enhanced leaderboard entry with rich information.
    /// </summary>
    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public long Score { get; set; }
        public string ScoreFormatted { get; set; } = string.Empty; // "1,234 XP", "42.5 km"
        public string BadgeTitle { get; set; } = string.Empty; // User's selected title
        public int Level { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public bool IsCurrentUser { get; set; }
        public bool IsFriend { get; set; }
        public bool IsClubMember { get; set; }
        public string TierName { get; set; } = string.Empty; // Bronze, Silver, Gold, etc.
        public string TierColor { get; set; } = string.Empty;
        public int TrendDirection { get; set; } // -1 down, 0 same, 1 up
        public int? PreviousRank { get; set; }
        public DateTime? LastActive { get; set; }
        public bool IsOnline { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<string> RecentBadges { get; set; } = new();
    }

    /// <summary>
    /// Current user's position details.
    /// </summary>
    public class UserLeaderboardPositionDto
    {
        public int Rank { get; set; }
        public long Score { get; set; }
        public string ScoreFormatted { get; set; } = string.Empty;
        public int? PreviousRank { get; set; }
        public int RankChange { get; set; } // +/- from previous
        public string TierName { get; set; } = string.Empty;
        public long PointsToNextRank { get; set; }
        public long PointsToNextTier { get; set; }
        public double PercentileRank { get; set; } // Top X%
        public List<string> Achievements { get; set; } = new(); // Recent leaderboard achievements
        public string PersonalizedGoal { get; set; } = string.Empty; // "Reach top 100"
    }

    /// <summary>
    /// Leaderboard statistics and insights.
    /// </summary>
    public class LeaderboardStatsDto
    {
        public long TopScore { get; set; }
        public long AverageScore { get; set; }
        public long MedianScore { get; set; }
        public long YourPercentile { get; set; }
        public int NewEntriesThisWeek { get; set; }
        public int ActiveParticipants { get; set; }
        public List<CompetitionInsightDto> Insights { get; set; } = new();
        public string TrendAnalysis { get; set; } = string.Empty;
    }

    /// <summary>
    /// Insights about competition and trends.
    /// </summary>
    public class CompetitionInsightDto
    {
        public string Type { get; set; } = string.Empty; // CloseCompetition, TrendingUp, etc.
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionSuggestion { get; set; } = string.Empty;
        public int Priority { get; set; }
    }

    /// <summary>
    /// Leaderboard tier information (Bronze, Silver, Gold, etc.).
    /// </summary>
    public class LeaderboardTierDto
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public long MinScore { get; set; }
        public long MaxScore { get; set; }
        public int ParticipantCount { get; set; }
        public double Percentile { get; set; }
        public List<string> Rewards { get; set; } = new();
        public bool IsCurrentUserTier { get; set; }
    }
}
