using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Domain.Entities.Gamification
{
    /// <summary>
    /// UserBadge - represents when and how a user earned a specific badge.
    /// Think of this as a "trophy case entry" - it records not just what was achieved,
    /// but when it happened and any special context around the achievement.
    /// </summary>
    public class UserBadge : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; }
        public int BadgeId { get; private set; }
        public Badge Badge { get; private set; }
        public DateTime EarnedAt { get; private set; }
        public string? EarnedContext { get; private set; } // JSON with details about how it was earned
        public bool IsVisible { get; private set; } // User can choose to hide certain badges

        private UserBadge() { } // For EF Core

        public UserBadge(User user, Badge badge, string? earnedContext = null)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            Badge = badge ?? throw new ArgumentNullException(nameof(badge));
            BadgeId = badge.Id;
            EarnedAt = DateTime.UtcNow;
            EarnedContext = earnedContext;
            IsVisible = true;

            // Award the XP for earning this badge
            user.AddExperience(badge.Points);
        }

        public void Hide()
        {
            IsVisible = false;
            MarkAsUpdated();
        }

        public void Show()
        {
            IsVisible = true;
            MarkAsUpdated();
        }

        public void UpdateContext(string earnedContext)
        {
            EarnedContext = earnedContext;
            MarkAsUpdated();
        }
    }
}
