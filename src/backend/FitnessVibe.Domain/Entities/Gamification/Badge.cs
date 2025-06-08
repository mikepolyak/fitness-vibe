using System;
using FitnessVibe.Domain.Common;

namespace FitnessVibe.Domain.Entities.Gamification
{
    /// <summary>
    /// Badge entity - like merit badges in scouting or achievements in video games.
    /// Each badge represents a milestone or accomplishment in the user's fitness journey.
    /// Think of badges as digital trophies that tell the story of what a user has achieved.
    /// </summary>
    public class Badge : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string IconUrl { get; private set; }
        public BadgeCategory Category { get; private set; }
        public BadgeRarity Rarity { get; private set; }
        public int Points { get; private set; } // XP awarded when earned
        public string Criteria { get; private set; } // JSON criteria for earning this badge
        public bool IsActive { get; private set; }

        private Badge() { } // For EF Core

        public Badge(
            string name,
            string description,
            string iconUrl,
            BadgeCategory category,
            BadgeRarity rarity,
            int points,
            string criteria)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            IconUrl = iconUrl ?? throw new ArgumentNullException(nameof(iconUrl));
            Category = category;
            Rarity = rarity;
            Points = points >= 0 ? points : throw new ArgumentException("Points cannot be negative");
            Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
            IsActive = true;
        }

        public void UpdateDetails(string name, string description, string iconUrl)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            IconUrl = iconUrl ?? throw new ArgumentNullException(nameof(iconUrl));
            MarkAsUpdated();
        }

        public void UpdateCriteria(string criteria)
        {
            Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }
    }

    public enum BadgeCategory
    {
        Activity,      // For completing activities
        Streak,        // For maintaining streaks
        Social,        // For social interactions
        Challenge,     // For completing challenges
        Milestone,     // For reaching milestones
        Special,       // For special events
        Achievement    // For general achievements
    }

    public enum BadgeRarity
    {
        Common,    // Easy to earn, frequent
        Uncommon,  // Moderate effort required
        Rare,      // Significant achievement
        Epic,      // Major accomplishment
        Legendary  // Extraordinary achievement
    }
}
