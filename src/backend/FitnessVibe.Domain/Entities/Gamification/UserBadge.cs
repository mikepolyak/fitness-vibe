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
        /// <summary>
        /// Gets the ID of the user who earned this badge
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the user who earned this badge
        /// </summary>
        public virtual User User { get; private set; } = null!;

        /// <summary>
        /// Gets the ID of the badge that was earned
        /// </summary>
        public Guid BadgeId { get; private set; }

        /// <summary>
        /// Gets the badge that was earned
        /// </summary>
        public virtual Badge Badge { get; private set; } = null!;

        /// <summary>
        /// Gets the UTC timestamp when the badge was earned
        /// </summary>
        public DateTime EarnedAt { get; private set; }

        /// <summary>
        /// Gets any special context about how the badge was earned (JSON)
        /// </summary>
        public string? EarnedContext { get; private set; }

        /// <summary>
        /// Gets whether this badge is visible on the user's profile
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected UserBadge() { }

        /// <summary>
        /// Creates a new UserBadge instance
        /// </summary>
        /// <param name="user">The user earning the badge</param>
        /// <param name="badge">The badge being earned</param>
        /// <param name="earnedContext">Optional context about how it was earned</param>
        /// <returns>A new UserBadge instance</returns>
        public static UserBadge Create(User user, Badge badge, string? earnedContext = null)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (badge == null)
                throw new ArgumentNullException(nameof(badge));

            var userBadge = new UserBadge
            {
                User = user,
                UserId = user.Id,
                Badge = badge,
                BadgeId = badge.Id,
                EarnedAt = DateTime.UtcNow,
                EarnedContext = earnedContext,
                IsVisible = true
            };

            // Award the XP for earning this badge
            user.AddExperience(badge.Points);

            return userBadge;
        }

        /// <summary>
        /// Makes this badge invisible on the user's profile
        /// </summary>
        public void Hide()
        {
            if (!IsVisible)
                return;

            IsVisible = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Makes this badge visible on the user's profile
        /// </summary>
        public void Show()
        {
            if (IsVisible)
                return;

            IsVisible = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the context information about how this badge was earned
        /// </summary>
        /// <param name="earnedContext">New context information (JSON)</param>
        public void UpdateContext(string? earnedContext)
        {
            EarnedContext = earnedContext;
            MarkAsUpdated();
        }
    }
}
