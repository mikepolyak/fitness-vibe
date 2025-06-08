using MediatR;

namespace FitnessVibe.Application.Commands.Gamification
{
    /// <summary>
    /// Command to award experience points to a user.
    /// Like giving bonus points for completing a challenging workout or hitting a milestone!
    /// </summary>
    public class AwardXpCommand : IRequest<AwardXpResponse>
    {
        public int UserId { get; set; }
        public int XpAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // Activity, Challenge, Social, etc.
        public int? SourceId { get; set; } // ID of the activity, challenge, etc.
        public Dictionary<string, object> Metadata { get; set; } = new();
        public bool NotifyUser { get; set; } = true;
        public int? MultiplierPercentage { get; set; } // Bonus multiplier (e.g., 150% = 50% bonus)
    }

    /// <summary>
    /// Response after awarding XP, including any level-ups or bonuses.
    /// Like getting your reward notification after earning achievement points!
    /// </summary>
    public class AwardXpResponse
    {
        public int XpAwarded { get; set; }
        public int BonusXp { get; set; }
        public int TotalXpAwarded { get; set; }
        public int NewTotalXp { get; set; }
        public int CurrentLevel { get; set; }
        public bool LeveledUp { get; set; }
        public int? NewLevel { get; set; }
        public string? NewLevelTitle { get; set; }
        public List<string> UnlockedFeatures { get; set; } = new();
        public List<BadgeEarnedInfo> BadgesEarned { get; set; } = new();
        public string CelebrationMessage { get; set; } = string.Empty;
        public int XpToNextLevel { get; set; }
    }

    /// <summary>
    /// Information about a badge that was earned.
    /// </summary>
    public class BadgeEarnedInfo
    {
        public int BadgeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int XpValue { get; set; }
        public bool IsFirstTime { get; set; }
    }
}
