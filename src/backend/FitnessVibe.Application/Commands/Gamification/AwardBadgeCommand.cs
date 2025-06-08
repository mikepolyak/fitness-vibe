using MediatR;

namespace FitnessVibe.Application.Commands.Gamification
{
    /// <summary>
    /// Command to award a badge to a user for their achievements.
    /// Like giving someone a trophy or medal for their fitness accomplishments!
    /// </summary>
    public class AwardBadgeCommand : IRequest<AwardBadgeResponse>
    {
        public int UserId { get; set; }
        public string BadgeCode { get; set; } = string.Empty; // Unique badge identifier
        public string Reason { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // Activity, Challenge, Achievement, etc.
        public int? SourceId { get; set; }
        public Dictionary<string, object> EvidenceData { get; set; } = new(); // Supporting data
        public bool NotifyUser { get; set; } = true;
        public bool NotifyFriends { get; set; } = true;
        public DateTime? EarnedAt { get; set; } // Defaults to now
    }

    /// <summary>
    /// Response after awarding a badge, including celebration details.
    /// Like the ceremony and recognition for earning an achievement!
    /// </summary>
    public class AwardBadgeResponse
    {
        public int UserBadgeId { get; set; }
        public BadgeDetailsDto Badge { get; set; } = new();
        public bool IsFirstTimeEarning { get; set; }
        public bool IsRareBadge { get; set; }
        public int XpBonusAwarded { get; set; }
        public string CelebrationMessage { get; set; } = string.Empty;
        public List<string> SocialShareSuggestions { get; set; } = new();
        public BadgeProgressDto? RelatedBadgeProgress { get; set; }
        public int BadgeCount { get; set; } // Total badges user now has
        public string BadgeRankTitle { get; set; } = string.Empty; // "Badge Collector", "Achievement Hunter", etc.
    }

    /// <summary>
    /// Detailed information about a badge.
    /// </summary>
    public class BadgeDetailsDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int XpValue { get; set; }
        public List<string> Requirements { get; set; } = new();
        public bool IsHidden { get; set; } // Hidden until earned
        public DateTime? EarnedAt { get; set; }
    }

    /// <summary>
    /// Progress towards earning a badge.
    /// </summary>
    public class BadgeProgressDto
    {
        public string BadgeCode { get; set; } = string.Empty;
        public string BadgeName { get; set; } = string.Empty;
        public double CurrentProgress { get; set; }
        public double RequiredProgress { get; set; }
        public double ProgressPercentage { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
        public string NextMilestone { get; set; } = string.Empty;
    }
}
