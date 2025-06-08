using MediatR;

namespace FitnessVibe.Application.Queries.Gamification
{
    /// <summary>
    /// Query to get a user's gamification dashboard with all achievements and progress.
    /// Like opening your trophy room to see all your fitness accomplishments and current goals!
    /// </summary>
    public class GetUserGamificationQuery : IRequest<UserGamificationResponse>
    {
        public int UserId { get; set; }
        public bool IncludeBadgeProgress { get; set; } = true;
        public bool IncludeRecentAchievements { get; set; } = true;
        public bool IncludeLeaderboardPosition { get; set; } = true;
        public bool IncludeNextMilestones { get; set; } = true;
    }

    /// <summary>
    /// Comprehensive gamification data for a user.
    /// Like a complete achievement report showing all your fitness gaming progress!
    /// </summary>
    public class UserGamificationResponse
    {
        public UserLevelInfo Level { get; set; } = new();
        public BadgeSummaryDto Badges { get; set; } = new();
        public List<RecentAchievementDto> RecentAchievements { get; set; } = new();
        public List<BadgeProgressDto> BadgeProgress { get; set; } = new();
        public List<MilestoneDto> UpcomingMilestones { get; set; } = new();
        public LeaderboardPositionDto LeaderboardPosition { get; set; } = new();
        public StreakInfoDto Streaks { get; set; } = new();
        public ChallengeProgressDto ActiveChallenges { get; set; } = new();
        public string MotivationalMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// User's level and XP information.
    /// Like your current rank and progress toward the next achievement tier!
    /// </summary>
    public class UserLevelInfo
    {
        public int CurrentLevel { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public int CurrentXp { get; set; }
        public int XpForCurrentLevel { get; set; }
        public int XpForNextLevel { get; set; }
        public int XpToNextLevel { get; set; }
        public double ProgressToNextLevel { get; set; }
        public string NextLevelTitle { get; set; } = string.Empty;
        public List<string> NextLevelUnlocks { get; set; } = new();
        public DateTime? LastLevelUpDate { get; set; }
        public int TotalXpEarned { get; set; }
    }

    /// <summary>
    /// Summary of user's badge collection.
    /// Like a showcase of all your achievement medals!
    /// </summary>
    public class BadgeSummaryDto
    {
        public int TotalBadges { get; set; }
        public int RecentBadges { get; set; } // Earned in last 30 days
        public List<BadgeDetailsDto> FeaturedBadges { get; set; } = new(); // Most impressive badges
        public List<BadgeDetailsDto> RecentlyEarned { get; set; } = new();
        public BadgeCategoryStatsDto CategoryStats { get; set; } = new();
        public int RankAmongFriends { get; set; }
        public string BadgeCollectorTitle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Statistics by badge category.
    /// </summary>
    public class BadgeCategoryStatsDto
    {
        public int ActivityBadges { get; set; }
        public int SocialBadges { get; set; }
        public int StreakBadges { get; set; }
        public int ChallengeBadges { get; set; }
        public int MilestoneBadges { get; set; }
        public int SpecialBadges { get; set; }
        public string StrongestCategory { get; set; } = string.Empty;
    }

    /// <summary>
    /// Recent achievement information.
    /// </summary>
    public class RecentAchievementDto
    {
        public string Type { get; set; } = string.Empty; // Badge, LevelUp, Milestone, etc.
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public DateTime AchievedAt { get; set; }
        public int XpEarned { get; set; }
        public bool IsRare { get; set; }
        public string ShareableText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Upcoming milestone information.
    /// </summary>
    public class MilestoneDto
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
        public double TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
        public int XpReward { get; set; }
        public string BadgeReward { get; set; } = string.Empty;
        public DateTime? EstimatedCompletionDate { get; set; }
        public int Priority { get; set; } // 1 = highest priority
    }

    /// <summary>
    /// User's position on various leaderboards.
    /// </summary>
    public class LeaderboardPositionDto
    {
        public int OverallRank { get; set; }
        public int XpRank { get; set; }
        public int ActivityRank { get; set; }
        public int BadgeRank { get; set; }
        public int FriendsRank { get; set; }
        public int LocalRank { get; set; } // Within city/region
        public string CurrentTier { get; set; } = string.Empty; // Bronze, Silver, Gold, etc.
        public List<LeaderboardEntryDto> NearbyCompetitors { get; set; } = new();
    }

    /// <summary>
    /// Leaderboard entry showing a competitor.
    /// </summary>
    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Score { get; set; }
        public string ScoreLabel { get; set; } = string.Empty; // "XP", "Activities", etc.
        public bool IsCurrentUser { get; set; }
        public bool IsFriend { get; set; }
    }

    /// <summary>
    /// Streak information for various activities.
    /// </summary>
    public class StreakInfoDto
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? StreakStartDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public List<StreakTypeDto> StreaksByType { get; set; } = new();
        public int DaysUntilStreakMilestone { get; set; }
        public string NextStreakReward { get; set; } = string.Empty;
    }

    /// <summary>
    /// Streak information for a specific activity type.
    /// </summary>
    public class StreakTypeDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastActivity { get; set; }
    }

    /// <summary>
    /// Active challenge progress.
    /// </summary>
    public class ChallengeProgressDto
    {
        public int ActiveChallenges { get; set; }
        public List<ActiveChallengeDto> Challenges { get; set; } = new();
        public int CompletedChallenges { get; set; }
        public int ChallengeWins { get; set; }
        public string FavoriteChallenge { get; set; } = string.Empty;
    }

    /// <summary>
    /// Information about an active challenge.
    /// </summary>
    public class ActiveChallengeDto
    {
        public int ChallengeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public double CurrentProgress { get; set; }
        public double TargetProgress { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
        public int CurrentPosition { get; set; }
        public int TotalParticipants { get; set; }
        public string RewardDescription { get; set; } = string.Empty;
    }
}
